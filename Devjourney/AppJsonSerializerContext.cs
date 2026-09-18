using System.Text.Json.Serialization;
using Application.Repositories;

namespace Devjourney
{
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        GenerationMode = JsonSourceGenerationMode.Default,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
    [JsonSerializable(typeof(Application.Modules.Competitions.Dtos.AvailableCompetitionDto))]
    [JsonSerializable(typeof(Application.Modules.Competitions.Dtos.CompetitionDetailsDto))]
    [JsonSerializable(typeof(Application.Modules.Competitions.Dtos.CompetitionStageDetailsDto))]
    [JsonSerializable(typeof(Application.Modules.Auth.Models.LoginResponseDto))]
    [JsonSerializable(typeof(Application.Modules.Skills.SkillDto))]
    [JsonSerializable(typeof(System.Collections.Generic.List<Application.Modules.Skills.SkillDto>))]
    [JsonSerializable(typeof(Application.Modules.Languages.LanguageDto))]
    [JsonSerializable(typeof(System.Collections.Generic.List<Application.Modules.Languages.LanguageDto>))]
    [JsonSerializable(typeof(Application.Modules.Roles.RoleDto))]
    [JsonSerializable(typeof(System.Collections.Generic.List<Application.Modules.Roles.RoleDto>))]
    [JsonSerializable(typeof(Application.Modules.IdeaFields.IdeaFieldDto))]
    [JsonSerializable(typeof(System.Collections.Generic.List<Application.Modules.IdeaFields.IdeaFieldDto>))]
    [JsonSerializable(typeof(Application.Modules.Professions.ProfessionDto))]
    [JsonSerializable(typeof(System.Collections.Generic.List<Application.Modules.Professions.ProfessionDto>))]
    [JsonSerializable(typeof(Application.Modules.MainRoles.MainRoleDto))]
    [JsonSerializable(typeof(System.Collections.Generic.List<Application.Modules.MainRoles.MainRoleDto>))]
    [JsonSerializable(typeof(Application.Modules.Student.Queries.GetStudentProfile.StudentProfileDto))]
    [JsonSerializable(typeof(Application.Modules.Student.Queries.GetAllStudentProfiles.StudentProfileListDto))]
    [JsonSerializable(typeof(Application.Modules.Student.Queries.GetStudentProfileCompletion.ProfileCompletionDto))]
    [JsonSerializable(typeof(Application.Modules.Jury.Queries.GetJuryProfile.JuryProfileDto))]
    [JsonSerializable(typeof(Application.Modules.University.Queries.GetUniversityProfile.UniversityProfileDto))]
    [JsonSerializable(typeof(Application.Repositories.PagedResult<Application.Modules.Competitions.Dtos.AvailableCompetitionDto>))]
    [JsonSerializable(typeof(Application.Repositories.PagedResult<Application.Modules.Student.Queries.GetAllStudentProfiles.StudentProfileListDto>))]
    [JsonSerializable(typeof(Application.Repositories.PagedResult<Application.Repositories.TeamMemberSearchPostPagedItemDto>))]
    [JsonSerializable(typeof(Application.Repositories.PagedResult<Application.Repositories.TeamSearchPostPagedItemDto>))]
    [JsonSerializable(typeof(Application.Repositories.PagedResult<Application.Modules.Posts.Queries.GetB2BCoursePromoPost.B2BCoursePromoPostPagedItemDto>))]
    [JsonSerializable(typeof(Application.Repositories.PagedResult<Application.Modules.Posts.Queries.GetCorporateEventPost.CorporateEventPostPagedItemDto>))]
    [JsonSerializable(typeof(Application.Repositories.PagedResult<Application.Modules.Posts.Queries.GetNetworkingEventPost.NetworkingEventPostPagedItemDto>))]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
