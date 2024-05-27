using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lemondo.Libary.API.Helpers;

public class FlagsEnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum && context.Type.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
        {
            schema.Type = "array";
            schema.Enum = context.Type.GetEnumNames()
            .Select(name => new OpenApiString(name))
            .Cast<IOpenApiAny>()
            .ToList();
        }
    }
}
