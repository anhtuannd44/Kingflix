using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kingflix.Website
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // routes.MapRoute(
            //     name: "Page",
            //     url: "trang/{url}",
            //     defaults: new { controller = "Page", action = "Index", id = UrlParameter.Optional }
            // );

            // //Blogs
            // routes.MapRoute(
            //     name: "BlogDetail",
            //     url: "blog/chi-tiet/{Url}",
            //     defaults: new { controller = "Blog", action = "Details", id = UrlParameter.Optional }
            // );

            // routes.MapRoute(
            //    name: "BlogCategory",
            //    url: "blog/{url}",
            //    defaults: new { controller = "Blog", action = "BlogList", id = UrlParameter.Optional }
            //);

            // routes.MapRoute(
            //     name: "BlogIndex",
            //     url: "blog",
            //     defaults: new { controller = "Blog", action = "Index", id = UrlParameter.Optional }
            // );

            // //Jobs
            // routes.MapRoute(
            //     name: "JobsDetail",
            //     url: "tuyen-dung/chi-tiet/{Url}",
            //     defaults: new { controller = "Jobs", action = "Details", id = UrlParameter.Optional }
            // );

            // routes.MapRoute(
            //    name: "JobsCategory",
            //    url: "tuyen-dung/{url}",
            //    defaults: new { controller = "Jobs", action = "JobList", id = UrlParameter.Optional }
            //);

            // routes.MapRoute(
            //     name: "JobsIndex",
            //     url: "tuyen-dung",
            //     defaults: new { controller = "Jobs", action = "Index", id = UrlParameter.Optional }
            // );

            // routes.MapRoute(
            //    name: "BaoTri",
            //    url: "bao-tri",
            //    defaults: new { area = "Admin", controller = "BaoTri", action = "Index" }
            //);

            routes.MapRoute(
                 "",
                 "{controller}/{action}/{id}",
                 new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                 new string[] { "Kingflix.Website.Controllers" }
                 );
        }
    }
}
