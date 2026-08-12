using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Devjourney.Filters
{
    public class CleanMediaTypesOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.RequestBody?.Content != null)
            {
                var keysToRemove = operation.RequestBody.Content.Keys
                    .Where(k => k != "application/json")
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    operation.RequestBody.Content.Remove(key);
                }
            }
        }
    }
}
