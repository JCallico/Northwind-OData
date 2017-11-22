using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.OData;
using System.Web.Http;

using GSA.Samples.Northwind.OData.Models;
using GSA.Samples.Northwind.OData.Filters;

namespace GSA.Samples.Northwind.OData.Controllers
{
    [KeyAndSecretBasicAuthentication] // Enable authentication via an ASP.NET Identity user name and password
    [Authorize] // Require some form of authentication
    public class BaseController<E, TKey> : ODataController where E : class, IODataEntity<TKey>
    {
        private IDbConnection _db = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindContext"].ConnectionString);

        protected virtual IDbConnection Db
        {
            get
            {
                return _db;
            }
        }

        protected override void Dispose(bool disposing)
        {
            Db.Dispose();
            base.Dispose(disposing);
        }
    }
}