using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using Swashbuckle.Swagger;
using IOperationFilter = Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter;
using System.Linq;
using ApiDescription = System.Web.Http.Description.ApiDescription;
using Parameter = Swashbuckle.Swagger.Parameter;
using System;

namespace PimFullItemCatalog.Swagger
{
    public class SwaggerParameterOperationFilter : IOperationFilter
    {
        public void Apply(Swashbuckle.Swagger.Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var requestAttributes = apiDescription.GetControllerAndActionAttributes<SwaggerParameterAttribute>();
            if (requestAttributes.Any())
            {
                operation.parameters = operation.parameters ?? new List<Swashbuckle.Swagger.Parameter>();

                foreach (var attr in requestAttributes)
                {
                    operation.parameters.Add(new Parameter
                    {
                        name = attr.Name,
                        description = attr.Description,
                        @in = attr.Type == "file" ? "formData" : "body",
                        required = attr.Required,
                        type = attr.Type
                    });
                }

                if (requestAttributes.Any(x => x.Type == "file"))
                {
                    operation.consumes.Add("multipart/form-data");
                }
            }
        }

        void IOperationFilter.Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
