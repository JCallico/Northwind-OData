using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.OData;
using System.Web.Http;

using GSA.Samples.Northwind.OData.Common.Filters;
using GSA.Samples.Northwind.OData.Model;

namespace GSA.Samples.Northwind.OData.Controllers
{
    [KeyAndSecretBasicAuthentication] // Enable authentication via an ASP.NET Identity user name and password
    [Authorize] // Require some form of authentication
    public class BaseController<TEntity, TKey> : ODataController where TEntity : class, IODataEntity<TEntity, TKey>
    {
        protected virtual IDbConnection Db { get; } = new SqlConnection(ConfigurationManager.ConnectionStrings["NorthwindContext"].ConnectionString);

        protected override void Dispose(bool disposing)
        {
            Db.Dispose();
            base.Dispose(disposing);
        }
    }
}