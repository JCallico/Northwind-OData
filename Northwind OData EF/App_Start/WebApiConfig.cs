using System.Web.Http;
using System.Web.OData.Extensions;
using GSA.Samples.Northwind.OData.Model;
using TraceLevel = System.Web.Http.Tracing.TraceLevel;

namespace GSA.Samples.Northwind.OData
{
    using System;
    using System.Configuration;

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

            if (bool.Parse(ConfigurationManager.AppSettings["SystemDiagnostics.Tracing.Enabled"] ?? "false"))
            {
                var traceWriter = config.EnableSystemDiagnosticsTracing();
                traceWriter.IsVerbose = bool.Parse(ConfigurationManager.AppSettings["SystemDiagnostics.Tracing.Verbose"] ?? "false");
                traceWriter.MinimumLevel = (TraceLevel)Enum.Parse(typeof(TraceLevel), ConfigurationManager.AppSettings["SystemDiagnostics.Tracing.MinimumLevel"] ?? "Debug");
            }
        }
    }
}
