using System.Web.OData.Builder;
using Microsoft.OData.Edm;

namespace GSA.Samples.Northwind.OData.Model
{
    public partial class NorthwindContext
    {
        public static IEdmModel GetConventionModel()
        {
            ODataModelBuilder builder = new ODataConventionModelBuilder();

            builder.Namespace = "Northwind";

            builder.ContainerName = "NorthwindContainer";

            builder.EntitySet<Category>("Categories");

            builder.EntitySet<Customer>("Customers");

            var entitySet = builder.EntitySet<Product>("Products");
            entitySet.EntityType.HasKey(x => x.ID);

            builder.EntitySet<Order>("Orders");

            return builder.GetEdmModel();
        }
    }
}