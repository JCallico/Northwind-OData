using System.Web.Http;
using System.Web.OData.Extensions;
using GSA.Samples.Northwind.OData.Model;
using TraceLevel = System.Web.Http.Tracing.TraceLevel;

namespace GSA.Samples.Northwind.OData
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // enabling querying options
            config.Select().Expand().Filter().OrderBy().MaxTop(null).Count();

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
