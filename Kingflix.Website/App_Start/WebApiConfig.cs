using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Kingflix.Website.CustomFilters;
using Kingflix.Website.Extensions.WebApi;

namespace Kingflix.Website
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Filters.Add(new WebApiExceptionFilterAttribute());
            config.Services.Replace(typeof(IExceptionHandler), new WebApiGlobalExceptionHandler());
            config.Services.Add(typeof(IExceptionLogger), new WebApiGlobalErrorLogger());

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}"
            );
        }
    }
}
