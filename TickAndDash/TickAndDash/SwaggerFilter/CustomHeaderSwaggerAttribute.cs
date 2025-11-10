using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace TickAndDash.SwaggerFilter
{
    public class CustomHeaderSwaggerAttribute : IOperationFilter
    {
        private string _xAPIVersioning;
        private string _description;

        public CustomHeaderSwaggerAttribute(string xAPIVersioning, string description)
        {
            if (string.IsNullOrWhiteSpace(xAPIVersioning))
                throw new ArgumentNullException(nameof(xAPIVersioning));
            else if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));


            _xAPIVersioning = xAPIVersioning;
            _description = description;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "x-api-version",
                In = ParameterLocation.Header,
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "String"
                },
                Example = new OpenApiString(_xAPIVersioning),
                Description = _description   //""
            }); ;
        }

    }
}
