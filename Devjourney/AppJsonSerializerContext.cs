using System.Text.Json.Serialization;
using Application.Repositories;

namespace Devjourney
{
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        GenerationMode = JsonSourceGenerationMode.Default,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonSerializable(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails))]
    // Add additional DTOs here progressively, e.g. [JsonSerializable(typeof(PagedResult<MyDto>))]
    public partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
