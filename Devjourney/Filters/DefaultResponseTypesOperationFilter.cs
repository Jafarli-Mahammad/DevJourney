using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Devjourney.Filters
{
    public class DefaultResponseTypesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!operation.Responses.ContainsKey("400"))
            {
                operation.Responses.Add("400", new OpenApiResponse
                {
                    Description = "Bad Request / Validation Error"
                });
            }

            if ((context.ApiDescription.HttpMethod == "POST" || context.ApiDescription.HttpMethod == "PUT" || context.ApiDescription.HttpMethod == "PATCH") && !operation.Responses.ContainsKey("415"))
            {
                operation.Responses.Add("415", new OpenApiResponse
                {
                    Description = "Unsupported Media Type"
                });
            }

            if (context.ApiDescription.ParameterDescriptions.Any(p => p.Source.Id == "Path") && !operation.Responses.ContainsKey("404"))
            {
                operation.Responses.Add("404", new OpenApiResponse
                {
                    Description = "Resource Not Found"
                });
            }
        }
    }
}
