using System;
using Swashbuckle.Swagger;

namespace GSA.Samples.Northwind.OData.Swagger
{
    /// <summary>
    /// Schema filters for Swagger library.
    /// </summary>
    public class SwaggerSchemaFilters : ISchemaFilter
    {
        public virtual void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
        {
            var dictionary = SwaggerFactory.CurrentFactory.CreateDataDictionary();
            var defaultValue = dictionary.GetDefaultValue(type);
            if (defaultValue != null)
            {
                schema.example = defaultValue;
            }
        }
    }
}
