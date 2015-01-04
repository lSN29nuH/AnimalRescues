﻿using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CritterHeroes.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DIConfig.ConfigureDependencyContainer();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DefaultHtmlConventions.AddHtmlConventions();

            ViewEngineConfig.ConfigureViewEngines(ViewEngines.Engines);
        }
    }
}