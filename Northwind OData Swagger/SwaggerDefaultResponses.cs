using System;
using System.Collections.Generic;
using System.Web.Http.Description;
using GSA.Samples.Northwind.OData.Model;
using Swashbuckle.Swagger;

namespace GSA.Samples.Northwind.OData.Swagger
{
    /// <summary>
    /// Default responses for Swagger library.
    /// </summary>
    public class SwaggerDefaultResponses : IOperationFilter
    {
        public virtual void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            switch (operation.operationId)
            {
                case "Categories_Post":
                    operation.responses["200"] = RegisterAndGetResponse(schemaRegistry, "200", "Success", typeof(Category));
                    break;

                case "Products_Post":
                    operation.responses["200"] = RegisterAndGetResponse(schemaRegistry, "200", "Success", typeof(Product));
                    break;

                    //default:
                    //    operation.responses["200"] = RegisterAndGetResponse(schemaRegistry, "200", "Success", typeof(ResponseMessage));
                    //    break;
            }

            //operation.responses["401"] = RegisterAndGetResponse(schemaRegistry, "401", "Unauthorized", typeof(UnauthorizedResponseMessage));
        }

        protected virtual Response RegisterAndGetResponse(SchemaRegistry schemaRegistry, string responseCode, string responseDescription, Type responseType)
        {
            schemaRegistry.GetOrRegister(responseType);

            var dictionary = SwaggerFactory.CurrentFactory.CreateDataDictionary();

            return new Response
            {
                description = responseDescription,
                schema = new Schema { @ref = "#/definitions/" + responseType.Name },
                examples = new Dictionary<string, object>
                {
                    {
                        "application/json", dictionary.GetValue(responseType, responseCode)
                    }
                }
            };
        }
    }
}
