# Devjourney post endpoints verification suite
# Run: powershell -File wwwroot/qa/verify-posts.ps1
$ErrorActionPreference = "Continue"
$Base = if ($env:API_BASE) { $env:API_BASE.TrimEnd("/") } else { "http://localhost:5074" }
$Results = [System.Collections.Generic.List[object]]::new()
$MissingGuid = [guid]::NewGuid().ToString()

function Add-Result($Type, $Case, $Pass, $Detail) {
    $Results.Add([pscustomobject]@{ Type = $Type; Case = $Case; Result = $(if ($Pass) { "PASS" } else { "FAIL" }); Detail = $Detail })
}

function Invoke-Api {
    param(
        [string]$Method,
        [string]$Path,
        [object]$Body = $null,
        [hashtable]$Headers = @{},
        [hashtable]$Query = @{}
    )
    $uri = "$Base$Path"
    if ($Query.Count) {
        $qs = [System.Web.HttpUtility]::ParseQueryString("")
        # fallback without System.Web
    }
    $ub = [System.UriBuilder]$uri
    if ($Query.Count) {
        $parts = @()
        foreach ($k in $Query.Keys) {
            $v = $Query[$k]
            if ($null -eq $v -or $v -eq "") { continue }
            if ($v -is [System.Array]) {
                foreach ($item in $v) { $parts += ("{0}={1}" -f [uri]::EscapeDataString($k), [uri]::EscapeDataString([string]$item)) }
            } else {
                $parts += ("{0}={1}" -f [uri]::EscapeDataString($k), [uri]::EscapeDataString([string]$v))
            }
        }
        if ($parts.Count) { $ub.Query = ($parts -join "&") }
    }
    $hdr = @{ Accept = "application/json" } + $Headers
    $params = @{
        Method      = $Method
        Uri         = $ub.Uri.AbsoluteUri
        Headers     = $hdr
        TimeoutSec  = 60
    }
    if ($null -ne $Body) {
        $params.ContentType = "application/json"
        $params.Body = ($Body | ConvertTo-Json -Depth 20 -Compress)
    }
    try {
        $resp = Invoke-WebRequest @params -UseBasicParsing
        $parsed = $null
        try { $parsed = $resp.Content | ConvertFrom-Json } catch { $parsed = $resp.Content }
        return [pscustomobject]@{
            Status  = [int]$resp.StatusCode
            Headers = $resp.Headers
            Body    = $parsed
            Raw     = $resp.Content
            Error   = $null
        }
    } catch {
        $ex = $_.Exception
        $status = 0
        $raw = $null
        $parsed = $null
        $hdrs = $null
        if ($ex.Response) {
            $status = [int]$ex.Response.StatusCode
            try {
                $stream = $ex.Response.GetResponseStream()
                $reader = New-Object System.IO.StreamReader($stream)
                $raw = $reader.ReadToEnd()
                try { $parsed = $raw | ConvertFrom-Json } catch { $parsed = $raw }
            } catch {}
            try { $hdrs = $ex.Response.Headers } catch {}
        } else {
            $raw = $ex.Message
        }
        return [pscustomobject]@{
            Status  = $status
            Headers = $hdrs
            Body    = $parsed
            Raw     = $raw
            Error   = $ex.Message
        }
    }
}

function Trunc([string]$s, $n = 400) {
    if ($null -eq $s) { return "" }
    if ($s.Length -le $n) { return $s }
    return $s.Substring(0, $n) + "..."
}

function Test-PostType {
    param(
        [string]$Type,
        [string]$ListPath,
        [string]$DetailPathTemplate, # e.g. /api/posts/x/{id}
        [scriptblock]$ValidBody,
        [scriptblock]$InvalidBody,
        [hashtable]$MeaningfulFilter,
        [hashtable]$NoMatchFilter,
        [scriptblock]$FilterMatcher, # ($item, $filter) -> bool
        [string]$FilterFieldLabel,
        [string[]]$NestedProps # detail nested collection property names (camelCase)
    )

    Write-Host "`n=== $Type ===" -ForegroundColor Cyan

    # --- POST create authenticated ---
    $authNote = "No login/JWT/seeded users in repo (JwtService NotImplemented, AuthController has register only). Cannot obtain JWT for seeded account."
    Add-Result $Type "POST create authenticated (201 + Location resolves)" $false $authNote

    # --- POST without Authorization ---
    $body = & $ValidBody
    $r = Invoke-Api -Method POST -Path $ListPath -Body $body
    $pass401 = ($r.Status -eq 401)
    Add-Result $Type "POST without Authorization => 401" $pass401 ("status=$($r.Status) body=$(Trunc ($r.Raw))" )

    # --- POST invalid payload ---
    $inv = & $InvalidBody
    # Still no auth — expect 401 before validation; note if we somehow get further
    $rInv = Invoke-Api -Method POST -Path $ListPath -Body $inv
    if ($rInv.Status -eq 401) {
        Add-Result $Type "POST invalid payload => 400 validation" $false ("Blocked by [Authorize] before validation: status=401 body=$(Trunc ($rInv.Raw)). Cannot reach FluentValidation without auth.")
    } elseif ($rInv.Status -eq 400) {
        Add-Result $Type "POST invalid payload => 400 validation" $true ("status=400 body=$(Trunc ($rInv.Raw))")
    } else {
        Add-Result $Type "POST invalid payload => 400 validation" $false ("status=$($rInv.Status) body=$(Trunc ($rInv.Raw))")
    }

    # Corporate verified-author special case handled outside for Corporate only

    # --- GET paged no filters ---
    $rAll = Invoke-Api -Method GET -Path $ListPath -Query @{ Page = 1; PageSize = 100 }
    $allOk = ($rAll.Status -eq 200 -and $null -ne $rAll.Body)
    $total = if ($allOk) { [int]$rAll.Body.totalCount } else { -1 }
    $items = if ($allOk -and $rAll.Body.items) { @($rAll.Body.items) } else { @() }
    $seedNote = if ($total -eq 0) { " totalCount=0 (no seeded posts in DB)" } else { " totalCount=$total items=$($items.Count)" }
    Add-Result $Type "GET paged no filters (200 + seeded count)" $allOk ("status=$($rAll.Status)$seedNote raw=$(Trunc ($rAll.Raw))")

    # --- GET one meaningful filter ---
    if (-not $allOk) {
        Add-Result $Type "GET filter $FilterFieldLabel subset match" $false "List endpoint failed: status=$($rAll.Status)"
        Add-Result $Type "GET filter no-match Guid => empty 200" $false "List endpoint failed"
        Add-Result $Type "GET page=2 pagination integrity" $false "List endpoint failed"
    } else {
        $rFilt = Invoke-Api -Method GET -Path $ListPath -Query ($MeaningfulFilter + @{ Page = 1; PageSize = 100 })
        if ($rFilt.Status -ne 200) {
            Add-Result $Type "GET filter $FilterFieldLabel subset match" $false ("status=$($rFilt.Status) body=$(Trunc ($rFilt.Raw))")
        } else {
            $fItems = @($rFilt.Body.items)
            $allMatch = $true
            $matchDetails = @()
            foreach ($it in $fItems) {
                $ok = & $FilterMatcher $it $MeaningfulFilter
                if (-not $ok) { $allMatch = $false; $matchDetails += (Trunc ($it | ConvertTo-Json -Compress)) }
            }
            $isSubset = ($fItems.Count -le $items.Count) -and ($rFilt.Body.totalCount -le $total)
            # If unfiltered is empty, filtered empty with matching filter is vacuously ok for subset but not a real seed check
            $pass = ($allMatch -and $isSubset -and $rFilt.Status -eq 200)
            $extra = ""
            if ($total -eq 0) { $extra = " (vacuous: no seeded rows to filter)" }
            if (-not $allMatch) { $extra += " non-matching rows: $($matchDetails -join '; ')" }
            Add-Result $Type "GET filter $FilterFieldLabel subset match" $pass ("filteredCount=$($fItems.Count) unfiltered=$total$extra body=$(Trunc ($rFilt.Raw))")
        }

        $rNone = Invoke-Api -Method GET -Path $ListPath -Query ($NoMatchFilter + @{ Page = 1; PageSize = 10 })
        $emptyOk = ($rNone.Status -eq 200 -and [int]$rNone.Body.totalCount -eq 0 -and (@($rNone.Body.items).Count -eq 0))
        Add-Result $Type "GET filter no-match => empty 200" $emptyOk ("status=$($rNone.Status) totalCount=$($rNone.Body.totalCount) body=$(Trunc ($rNone.Raw))")

        # pagination page 2
        $pageSize = 1
        $r1 = Invoke-Api -Method GET -Path $ListPath -Query @{ Page = 1; PageSize = $pageSize }
        $r2 = Invoke-Api -Method GET -Path $ListPath -Query @{ Page = 2; PageSize = $pageSize }
        if ($total -lt 2) {
            Add-Result $Type "GET page=2 pagination integrity" $false ("Need >=2 seeded rows to force page 2; totalCount=$total p1=$(Trunc ($r1.Raw)) p2=$(Trunc ($r2.Raw))")
        } elseif ($r1.Status -ne 200 -or $r2.Status -ne 200) {
            Add-Result $Type "GET page=2 pagination integrity" $false ("p1=$($r1.Status) p2=$($r2.Status)")
        } else {
            $id1 = @($r1.Body.items | ForEach-Object { $_.id })
            $id2 = @($r2.Body.items | ForEach-Object { $_.id })
            $overlap = ($id1 | Where-Object { $id2 -contains $_ })
            $metaOk = ([int]$r1.Body.totalCount -eq $total) -and ([int]$r2.Body.totalCount -eq $total)
            $pass = ($overlap.Count -eq 0 -and $metaOk -and $id2.Count -gt 0)
            Add-Result $Type "GET page=2 pagination integrity" $pass ("total=$total overlap=$($overlap.Count) p1ids=$($id1 -join ',') p2ids=$($id2 -join ',')")
        }
    }

    # --- GET by id valid ---
    if ($items.Count -gt 0) {
        $id = $items[0].id
        $detailPath = $DetailPathTemplate.Replace("{id}", $id)
        $rd = Invoke-Api -Method GET -Path $detailPath
        if ($rd.Status -ne 200) {
            Add-Result $Type "GET by id seeded detail + nested" $false ("status=$($rd.Status) body=$(Trunc ($rd.Raw))")
        } else {
            $nestedOk = $true
            $nestedInfo = @()
            foreach ($np in $NestedProps) {
                $val = $rd.Body.$np
                $count = if ($null -eq $val) { -1 } elseif ($val -is [System.Array] -or $val -is [System.Collections.IEnumerable]) { @($val).Count } else { 1 }
                $nestedInfo += "$np=$count"
                if ($count -le 0) { $nestedOk = $false }
            }
            # TeamSearch returns domain entity; nested may be ideaFields
            Add-Result $Type "GET by id seeded detail + nested" $nestedOk ("status=200 nested: $($nestedInfo -join ', ') body=$(Trunc ($rd.Raw))")
        }
    } else {
        Add-Result $Type "GET by id seeded detail + nested" $false "No seeded/list items to fetch detail for"
    }

    # --- GET by id missing ---
    $missingPath = $DetailPathTemplate.Replace("{id}", $MissingGuid)
    $rm = Invoke-Api -Method GET -Path $missingPath
    $pass404 = ($rm.Status -eq 404)
    Add-Result $Type "GET by id missing => 404" $pass404 ("status=$($rm.Status) body=$(Trunc ($rm.Raw))")
}

# Probe list endpoints first to pick filter values from existing data when possible
$tmsList = Invoke-Api -Method GET -Path "/api/posts/team-member-search" -Query @{ Page = 1; PageSize = 50 }
$skills = Invoke-Api -Method GET -Path "/api/Lookups/skills"
$skillId = if ($skills.Body -and @($skills.Body).Count) { @($skills.Body)[0].id } else { $MissingGuid }
$ideaFieldId = $MissingGuid
if ($tmsList.Status -eq 200 -and @($tmsList.Body.items).Count -gt 0) {
    $ideaFieldId = @($tmsList.Body.items)[0].ideaFieldId
}

$dummyGuid = $MissingGuid
$now = [DateTime]::UtcNow
$later = $now.AddDays(1)

Test-PostType -Type "TeamMemberSearch" `
    -ListPath "/api/posts/team-member-search" `
    -DetailPathTemplate "/api/posts/team-member-search/{id}" `
    -ValidBody { @{
        ideaFieldId = $ideaFieldId; teamName = "QA Team"; membersNeeded = 2; workMode = 0
        socialLink = "https://t.me/qa"; lookingForAge = 20; lookingForLocation = "Baku"
        lookingForLanguageId = $null; additionalNote = "note"
        targetRoleIds = @($dummyGuid); targetSkillIds = @($skillId)
    } } `
    -InvalidBody { @{ ideaFieldId = $ideaFieldId; membersNeeded = 2; workMode = 0; socialLink = "x"; targetRoleIds = @($dummyGuid); targetSkillIds = @($skillId) } } `
    -MeaningfulFilter @{ IdeaFieldId = $ideaFieldId } `
    -FilterFieldLabel "IdeaFieldId" `
    -FilterMatcher { param($item,$f) $item.ideaFieldId -eq $f.IdeaFieldId } `
    -NestedProps @("targetRoles","targetSkills")

Test-PostType -Type "TeamSearch" `
    -ListPath "/api/posts/team-search" `
    -DetailPathTemplate "/api/posts/team-search/{id}" `
    -ValidBody { @{ note = "looking"; ideaFieldIds = @($ideaFieldId) } } `
    -InvalidBody { @{ note = "looking"; ideaFieldIds = $null } } `
    -MeaningfulFilter @{ IdeaFieldIds = @($ideaFieldId) } `
    -FilterFieldLabel "IdeaFieldIds" `
    -FilterMatcher { param($item,$f) $ids = @($item.ideaFieldIds); $ids -contains $f.IdeaFieldIds[0] } `
    -NestedProps @("ideaFields")

Test-PostType -Type "NetworkingEvent" `
    -ListPath "/api/posts/networking-events" `
    -DetailPathTemplate "/api/posts/networking-events/{id}" `
    -ValidBody { @{
        organizationName = "Org"; location = "Baku"; latitude = 40.4; longitude = 49.8
        startAt = $now.ToString("o"); endAt = $later.ToString("o"); maxAttendees = 50
        ticketContact = "t@t.com"; isPaid = $false; price = $null; stops = @("Stop A","Stop B")
    } } `
    -InvalidBody { @{ location = "Baku"; startAt = $now.ToString("o"); endAt = $later.ToString("o"); maxAttendees = 50; isPaid = $false; stops = @() } } `
    -MeaningfulFilter @{ Location = "Baku" } `
    -FilterFieldLabel "Location" `
    -FilterMatcher { param($item,$f) ($item.location -like "*$($f.Location)*") } `
    -NestedProps @("stops")

# Adjust networking filter if we have data
$neList = Invoke-Api -Method GET -Path "/api/posts/networking-events" -Query @{ Page = 1; PageSize = 1 }

Test-PostType -Type "CorporateEvent" `
    -ListPath "/api/posts/corporate-events" `
    -DetailPathTemplate "/api/posts/corporate-events/{id}" `
    -ValidBody { @{
        title = "Corp Event"; eventTypeId = $dummyGuid; specialOccasion = $null
        startAt = $now.ToString("o"); endAt = $later.ToString("o"); location = "Baku"
        latitude = $null; longitude = $null; targetAudience = "Students"; maxAttendees = 100
        confidentialityNote = $null; applicationMethod = 0; applicationLink = $null
        eventLanguage = "en"; isPaid = $false; price = $null
        agenda = @(@{ time = "10:00"; activity = "Intro" })
    } } `
    -InvalidBody { @{ eventTypeId = $dummyGuid; startAt = $now.ToString("o"); endAt = $later.ToString("o"); location = "Baku"; targetAudience = "x"; maxAttendees = 1; applicationMethod = 0; isPaid = $false } } `
    -MeaningfulFilter @{ EventTypeId = $dummyGuid } `
    -FilterFieldLabel "EventTypeId" `
    -FilterMatcher { param($item,$f) $item.eventTypeId -eq $f.EventTypeId } `
    -NestedProps @("agenda")

# Corporate VerifiedAuthor special case
Write-Host "`n=== CorporateEvent VerifiedAuthor ===" -ForegroundColor Cyan
Add-Result "CorporateEvent" "POST as unverified company1@seed.test => VerifiedAuthor reject" $false `
    "Cannot run: no login endpoint, no JWT, no company1@seed.test seeder in repo. VerifiedAuthorBehaviour would throw UnauthorizedException('Only verified companies can perform this action.') once auth exists."

Test-PostType -Type "B2BCoursePromo" `
    -ListPath "/api/posts/b2b-course-promos" `
    -DetailPathTemplate "/api/posts/b2b-course-promos/{id}" `
    -ValidBody { @{
        courseName = "Course"; title = "Promo"; courseTypeId = $dummyGuid; eventFormat = 1
        targetMajor = "CS"; startAt = $now.ToString("o"); endAt = $later.ToString("o")
        locationOrLink = "https://zoom"; maxAttendees = 30; durationInfo = "2h"
        isPaid = $true; price = 10; hasDiscount = $false; discountNote = $null; hasCertificate = $true
        content = "content"; applicationMethod = 0; applicationLink = $null
        instructorName = "Inst"; instructorLinkedIn = $null; instructorReviewsLink = $null
    } } `
    -InvalidBody { @{ title = "Promo"; courseTypeId = $dummyGuid; eventFormat = 1; targetMajor = "CS"; startAt = $now.ToString("o"); endAt = $later.ToString("o"); locationOrLink = "x"; maxAttendees = 1; isPaid = $false; hasDiscount = $false; content = "c"; applicationMethod = 0 } } `
    -MeaningfulFilter @{ CourseTypeId = $dummyGuid } `
    -FilterFieldLabel "CourseTypeId" `
    -FilterMatcher { param($item,$f) $item.courseTypeId -eq $f.CourseTypeId } `
    -NestedProps @("instructor")

# Print table
Write-Host "`n================ VERIFY TABLE ================`n"
$Results | Format-Table -AutoSize Type, Case, Result, Detail
$out = "c:\Users\user\source\repos\Devjourney\Devjourney\wwwroot\qa\verify-results.json"
$Results | ConvertTo-Json -Depth 5 | Set-Content $out -Encoding utf8
$pass = @($Results | Where-Object Result -eq "PASS").Count
$fail = @($Results | Where-Object Result -eq "FAIL").Count
Write-Host "PASS=$pass FAIL=$fail TOTAL=$($Results.Count) written to $out"
