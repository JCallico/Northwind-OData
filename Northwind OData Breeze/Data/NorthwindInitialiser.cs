using GSA.Samples.Northwind.OData.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace Northwind_OData_Breeze.Data
{
    public class NorthwindInitialiser: System.Data.Entity.CreateDatabaseIfNotExists<NorthwindContext>
    {
        public NorthwindInitialiser()
        {
        }

        protected override void Seed(NorthwindContext context)
        {
            var sqlSriptPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"App_Data\dbo.Products.data.sql");
            var script = File.ReadAllText(sqlSriptPath);
            context.Database.ExecuteSqlCommand(script);
        }
    }
}