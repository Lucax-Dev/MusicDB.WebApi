using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MusicDB.WebApi.Utils;

    public class SwaggerAutorizeOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var method = context.MethodInfo;

            var anonymous = method.IsDefined(typeof(AllowAnonymousAttribute), true)
                || (method.DeclaringType?.IsDefined(typeof(AllowAnonymousAttribute), true) ?? false);

            var authorize = method.IsDefined(typeof(AuthorizeAttribute), true)
                || (method.DeclaringType?.IsDefined(typeof(AuthorizeAttribute), true) ?? false);

            if (anonymous || !authorize)
                return;

            operation.Security ??= new List<OpenApiSecurityRequirement>();
            operation.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", context.Document)] =
                  new List<string>()
            });
        }
    }

