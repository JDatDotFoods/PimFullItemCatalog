using System;

namespace PimFullItemCatalog.Swagger
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SwaggerParameterAttribute : Attribute
    {
        public SwaggerParameterAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public string Type { get; set; } = "text";

        public bool Required { get; set; } = false;
    }
}
