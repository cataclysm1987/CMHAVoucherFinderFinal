using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CMHAVoucherFinder
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.RouteExistingFiles = true;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "About",
                "About",
                new {controller = "Home", action = "About"}
            );

            routes.MapRoute(
                "My Properties",                             
                "MyProperties",                            
                new { controller = "Properties", action = "Index" }  
            );

            routes.MapRoute(
                "Browse Properties",
                "BrowseProperties",
                new { controller = "Properties", action = "Browse" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
