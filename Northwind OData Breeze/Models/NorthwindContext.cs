using Northwind_OData_Breeze.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GSA.Samples.Northwind.OData.Models
{
    public class NorthwindContext : DbContext
    {
        private const string _contextName = "NorthwindContext";
        public static string ContextName { get { return _contextName; } }

        public NorthwindContext() : base(ContextName)
        {
            Database.SetInitializer(new NorthwindInitialiser());
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.Add(new ProductConfiguration());
        }

        public DbSet<Product> Products { get; set; }
    }
}