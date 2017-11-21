using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using Breeze.WebApi2;
using GSA.Samples.Northwind.OData.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Northwind_OData_Breeze.Controllers
{
    [BreezeController(AllowedQueryOptions = System.Web.Http.OData.Query.AllowedQueryOptions.All)]
    public class NorthwindController : ApiController
    {
        readonly EFContextProvider<NorthwindContext> _contextProvider = new EFContextProvider<NorthwindContext>();

        // ~/breeze/Northwind/Metadata 
        [HttpGet]
        public string Metadata()
        {
            return _contextProvider.Metadata();
        }

        // ~/breeze/Northwind/Products
        // ~/breeze/Northwind/Products?$filter=IsArchived eq false&$orderby=CreatedAt 
        [HttpGet]
        public IQueryable<Product> Products()
        {
            return _contextProvider.Context.Products;
        }

        // ~/breeze/Northwind/SaveChanges
        [HttpPost]
        public SaveResult SaveChanges(JObject saveBundle)
        {
            return _contextProvider.SaveChanges(saveBundle);
        }
    }
}