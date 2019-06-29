using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OCREngine.WebApi.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace OCREngine.WebApi.Swagger
{
    public class SwaggerOAuth2SecurityRequirements : IOperationFilter
    {
        public void Apply(Swashbuckle.AspNetCore.Swagger.Operation operation, OperationFilterContext context)
        {
            var filterDescriptors = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var authorizationRequirements = filterDescriptors.GetPolicyRequirements();
            var claimTypes = authorizationRequirements
                .OfType<ClaimsAuthorizationRequirement>()
                .Select(x => x.ClaimType)
                .ToList();

            if (claimTypes.Any())
            {
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
            {
                new Dictionary<string, IEnumerable<string>>()
                {
                    { "basic", claimTypes },
                    { "oauth2", claimTypes }
                }
            };

            }
        }
    }
}