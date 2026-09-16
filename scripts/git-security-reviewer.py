#!/usr/bin/env python3
"""
AI Pre-Commit Security Reviewer powered by local WhiteRabbitNeo via Ollama.
Dedicated to detecting security vulnerabilities, authorization issues (BOLA/BFLA),
hardcoded secrets, injection vectors, and untrusted data handling in .NET/C#.
"""

import os
import sys
import json
import fnmatch
import datetime
import subprocess
import urllib.request
import urllib.error

# Configuration
OLLAMA_ENDPOINT = os.environ.get("OLLAMA_ENDPOINT", "http://localhost:11434")
OLLAMA_MODEL = os.environ.get("OLLAMA_SECURITY_MODEL", "whiterabbit")
MAX_DIFF_CHARS = int(os.environ.get("MAX_DIFF_CHARS", "18000"))

# File exclusion patterns
IGNORE_PATTERNS = [
    "*.lock", "*-lock.json", "*-lock.yaml", "package-lock.json", "pnpm-lock.yaml", "yarn.lock", "Cargo.lock",
    "packages.lock.json", "*.min.js", "*.min.css", "*.map", "*.svg", "*.png", "*.jpg", "*.jpeg", "*.gif",
    "*.ico", "*.wasm", "*.dll", "*.exe", "*.so", "*.dylib", "*.db", "*.sqlite", "*.log", "*.suo",
    "*.DotSettings.user", "*.DotSettings", ".idea/*", ".vscode/*", "zap-report.html", "codebase-memory.db",
    "*.nupkg", "obj/*", "bin/*"
]

# ANSI Colors
CYAN = "\033[96m"
GREEN = "\033[92m"
YELLOW = "\033[93m"
RED = "\033[91m"
MAGENTA = "\033[95m"
BOLD = "\033[1m"
DIM = "\033[2m"
RESET = "\033[0m"


def is_ignored(filename: str) -> bool:
    basename = os.path.basename(filename)
    for pattern in IGNORE_PATTERNS:
        if fnmatch.fnmatch(filename, pattern) or fnmatch.fnmatch(basename, pattern):
            return True
    return False


def check_ollama_status() -> bool:
    try:
        req = urllib.request.Request(f"{OLLAMA_ENDPOINT}/api/tags", method="GET")
        with urllib.request.urlopen(req, timeout=2) as response:
            return response.status == 200
    except Exception:
        return False


def get_git_info() -> tuple[str, str, str]:
    """Returns (repo_root, current_branch, commit_author)"""
    try:
        root = subprocess.run(["git", "rev-parse", "--show-toplevel"], capture_output=True, text=True, check=True).stdout.strip()
        branch = subprocess.run(["git", "rev-parse", "--abbrev-ref", "HEAD"], capture_output=True, text=True, check=True).stdout.strip()
        author = subprocess.run(["git", "config", "user.name"], capture_output=True, text=True).stdout.strip() or "Developer"
        return root, branch, author
    except Exception:
        return ".", "unknown", "Developer"


def get_staged_diff() -> tuple[str, list[str]]:
    cmd_files = ["git", "diff", "--cached", "--name-only", "--diff-filter=ACMR"]
    res_files = subprocess.run(cmd_files, capture_output=True, text=True, check=True)
    staged_files = [f.strip() for f in res_files.stdout.splitlines() if f.strip()]

    candidate_files = [f for f in staged_files if not is_ignored(f)]

    if not candidate_files:
        return "", []

    cmd_diff = ["git", "diff", "--cached", "--unified=3", "--"] + candidate_files
    res_diff = subprocess.run(cmd_diff, capture_output=True, text=True, check=True)
    return res_diff.stdout, candidate_files


def build_system_prompt() -> str:
    return (
        "You are an elite DevSecOps Gatekeeper and Application Security Auditor.\n"
        "Your task is to analyze staged Git diffs strictly for genuine, exploitable security vulnerabilities and explicit secret leaks.\n\n"
        
        "### CONTEXT\n"
        "Stack: C#, ASP.NET Core (Identity), EF Core, MediatR, CQRS, Clean Architecture.\n\n"
        
        "### SECURITY REVIEW CRITERIA:\n"
        "1. Auth & BOLA (IDOR): Missing [Authorize] attributes, bypassing MediatR pipeline behaviors for auth, or missing tenant/user ownership validation before data access.\n"
        "2. Injection: Using EF Core 'FromSqlRaw' with string interpolation (SQLi), unvalidated file paths (Directory Traversal), or unsafe deserialization.\n"
        "3. Secrets: ACTUAL hardcoded production API keys, JWT symmetric keys, or DB passwords.\n"
        "4. Cryptography & Privacy: Custom crypto implementations, or logging plaintext PII/passwords.\n\n"
        
        "### FALSE POSITIVE SUPPRESSION (CRITICAL):\n"
        "- IGNORE dependency injection boilerplate, localhost URLs, and standard routing.\n"
        "- IGNORE references to secrets via IConfiguration or Environment variables (these are safe).\n"
        "- IGNORE test data, mock variables, and appsettings.Development.json configurations.\n"
        "- DO NOT hallucinate vulnerabilities. Assume standard ASP.NET Core secure defaults are active unless the diff explicitly overrides them.\n\n"
        
        "### INSTRUCTIONS\n"
        "1. Read the diff and identify the attack surface.\n"
        "2. Think step-by-step in the Threat Analysis section before concluding.\n"
        "3. If genuine, exploitable vulnerabilities or exposed production secrets exist, you MUST REJECT.\n"
        "4. If the code introduces no real attack surface, you MUST APPROVE.\n\n"
        
        "### OUTPUT FORMAT\n"
        "You must respond EXACTLY in the following Markdown structure:\n\n"
        
        "### 🛡️ Security Assessment Summary\n"
        "[1-2 sentences summarizing the attack surface changes]\n\n"
        
        "### ⚙️ Threat Analysis\n"
        "[Briefly model the threat. Think step-by-step: Is authorization enforced? Is input sanitized? Are secrets actually exposed?]\n\n"
        
        "### 🚨 Vulnerabilities\n"
        "- [Actionable bullet points of genuine exploits/leaks, or '- None identified']\n\n"
        
        "### 🔒 Hardening Suggestions\n"
        "- [Defense-in-depth suggestions, or '- None']\n\n"
        
        "### 🎯 Verdict\n"
        "[VERDICT: APPROVE] or [VERDICT: REJECT]"
    )


def stream_review_from_ollama(diff_text: str, files: list[str]) -> tuple[str, str]:
    truncated = False
    if len(diff_text) > MAX_DIFF_CHARS:
        diff_text = diff_text[:MAX_DIFF_CHARS] + "\n\n... [Diff truncated: exceeding max token limit for security review] ..."
        truncated = True

    system_prompt = build_system_prompt()
    user_prompt = (
        f"Staged Files ({len(files)}):\n" + "\n".join(f"- `{f}`" for f in files) +
        f"\n\n```diff\n{diff_text}\n```"
    )

    payload = {
        "model": OLLAMA_MODEL,
        "messages": [
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_prompt}
        ],
        "stream": True,
        "options": {
            "temperature": 0.1,
            "top_p": 0.85,
        }
    }

    req = urllib.request.Request(
        f"{OLLAMA_ENDPOINT}/api/chat",
        data=json.dumps(payload).encode("utf-8"),
        headers={"Content-Type": "application/json"}
    )

    full_response = ""
    print(f"\n{BOLD}{MAGENTA}🛡️  Security Agent ({OLLAMA_MODEL}) reviewing {len(files)} staged file(s)...{RESET}\n")
    if truncated:
        print(f"{DIM}(Note: Large diff truncated to first {MAX_DIFF_CHARS} chars){RESET}\n")

    try:
        with urllib.request.urlopen(req, timeout=120) as response:
            for line in response:
                if not line:
                    continue
                chunk = json.loads(line.decode("utf-8"))
                token = chunk.get("message", {}).get("content", "")
                sys.stdout.write(token)
                sys.stdout.flush()
                full_response += token
        print("\n")
    except Exception as e:
        print(f"\n{YELLOW}⚠️ Error during Ollama security review: {e}{RESET}\n")
        return "", "ERROR"

    verdict = "APPROVE"
    if "[VERDICT: REJECT]" in full_response or "VERDICT: REJECT" in full_response:
        verdict = "REJECT"
    elif "[VERDICT: APPROVE]" in full_response or "VERDICT: APPROVE" in full_response:
        verdict = "APPROVE"
    elif any(k in full_response.lower() for k in ["recommend rejecting", "reject this commit", "critical vulnerability", "sql injection", "idor vulnerability", "hardcoded credentials"]):
        verdict = "REJECT"

    return full_response, verdict


def save_security_report(repo_root: str, branch: str, files: list[str], review_text: str, verdict: str):
    git_dir = os.path.join(repo_root, ".git")
    if not os.path.exists(git_dir):
        return

    report_path = os.path.join(git_dir, "LAST_SECURITY_REPORT.md")
    now_str = datetime.datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    badge = "🟢 **PASSED (APPROVE)**" if verdict == "APPROVE" else "🔴 **REJECTED (SECURITY RISKS FOUND)**"

    report_content = f"""# 🛡️ Local Security Pre-Commit Report

- **Date:** `{now_str}`
- **Branch:** `{branch}`
- **Model:** `{OLLAMA_MODEL}`
- **Status:** {badge}

---

### 📂 Staged Files ({len(files)})
""" + "\n".join(f"- `{f}`" for f in files) + f"""

---

{review_text.strip()}

---
*Generated automatically by `.git/hooks/pre-commit` via Ollama.*
"""

    try:
        with open(report_path, "w", encoding="utf-8") as f:
            f.write(report_content)
        print(f"{DIM}📄 Full Security report saved to: .git/LAST_SECURITY_REPORT.md{RESET}")
    except Exception as e:
        print(f"{DIM}[Security Hook] Could not save report file: {e}{RESET}")


def prompt_user_confirmation(verdict: str) -> bool:
    if verdict == "APPROVE":
        print(f"{GREEN}{BOLD}✅ Security Verdict: APPROVED! No blocking security risks detected.{RESET}\n")
        return True

    print(f"{RED}{BOLD}🛑 Security Review REJECTED this commit based on risks above.{RESET}\n")
    tty_path = "/dev/tty"
    try:
        if os.path.exists(tty_path):
            with open(tty_path, "r") as tty_in, open(tty_path, "w") as tty_out:
                tty_out.write(f"{BOLD}Proceed with commit anyway? [y/N]: {RESET}")
                tty_out.flush()
                answer = tty_in.readline().strip().lower()
                if answer in ("y", "yes"):
                    tty_out.write(f"{YELLOW}Bypassing security rejection upon user request.{RESET}\n\n")
                    return True
                else:
                    tty_out.write(f"{RED}Commit aborted due to security rejection.{RESET}\n\n")
                    return False
    except Exception:
        pass

    # If non-interactive (no TTY available to prompt)
    print(f"{RED}Non-interactive session: Aborting commit due to security rejection.{RESET}\n")
    return False


def main():
    if os.environ.get("SKIP_SECURITY_REVIEW") == "1" or os.environ.get("SKIP_QA") == "1":
        return 0

    if not check_ollama_status():
        print(f"{DIM}[Security Hook] Ollama not reachable at {OLLAMA_ENDPOINT}. Skipping AI security check.{RESET}")
        return 0

    repo_root, branch, _ = get_git_info()

    try:
        diff_text, files = get_staged_diff()
    except subprocess.CalledProcessError as e:
        print(f"{YELLOW}[Security Hook] Could not read git diff: {e}{RESET}")
        return 0

    if not diff_text.strip() or not files:
        return 0

    response, verdict = stream_review_from_ollama(diff_text, files)
    if verdict == "ERROR":
        return 0

    save_security_report(repo_root, branch, files, response, verdict)

    proceed = prompt_user_confirmation(verdict)
    if not proceed:
        return 1

    return 0


if __name__ == "__main__":
    sys.exit(main())
