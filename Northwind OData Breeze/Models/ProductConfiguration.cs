using System.Data.Entity.ModelConfiguration;

namespace GSA.Samples.Northwind.OData.Models
{
    public class ProductConfiguration : EntityTypeConfiguration<Product>
    {
        public ProductConfiguration() 
        {
        }
    }
}