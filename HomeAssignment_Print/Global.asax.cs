using HomeAssignment.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HomeAssignment_Print
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AppInit();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            AppCleanUp();
        }

        private void AppInit()
        {
            Debug.Write("Application_Start");
            //TODO: us DI instead of manually instantiating service
            HttpContext.Current.Application["PrintService"] = new PrintService(Directory.GetCurrentDirectory() + "\\queue.json");
        }

        private void AppCleanUp()
        {
            Debug.Write("Application_End");
            //make sure the print service is disposed as expected
            if (HttpContext.Current.Application["PrintService"] != null)
            {
                PrintService temp = (PrintService)HttpContext.Current.Application["PrintService"];
                temp.Dispose();
            }
        }
    }
}
