using System;
using System.Web.Http;
using GSA.Samples.Northwind.OData;
using GSA.Samples.Northwind.OData.App_Start;

namespace Northwind_OData
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(EFConfig.Register);
        }
    }
}
