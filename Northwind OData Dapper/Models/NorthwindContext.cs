using System.Web.OData.Builder;
using GSA.Samples.Northwind.OData.Model;
using Microsoft.OData.Edm;

namespace GSA.Samples.Northwind.OData.Models
{
    public class NorthwindContext
    {
        public static IEdmModel GetConventionModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();
            
            builder.Namespace = "Northwind";

            builder.ContainerName = "NorthwindContainer";

            var entitySet = builder.EntitySet<Product>("Products");
            entitySet.EntityType.Select(nameof(Product.ID));
            entitySet.EntityType.Select(nameof(Product.ProductName));
            entitySet.EntityType.Select(nameof(Product.UnitPrice));
            entitySet.EntityType.Select(nameof(Product.ReferenceUniqueID));
            entitySet.EntityType.Select(nameof(Product.ProductUri));

            return builder.GetEdmModel();
        }
    }
}