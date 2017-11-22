using System.Web.Http;
using System.Web.OData.Extensions;
using GSA.Samples.Northwind.OData.Models;
using TraceLevel = System.Web.Http.Tracing.TraceLevel;

namespace GSA.Samples.Northwind.OData.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: null,
                model: NorthwindContext.GetConventionModel());

            var traceWriter = config.EnableSystemDiagnosticsTracing();
            traceWriter.IsVerbose = true;
            traceWriter.MinimumLevel = TraceLevel.Debug;
        }
    }
}
