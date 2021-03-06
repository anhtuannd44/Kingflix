using System.Web.Mvc;

namespace Kingflix.Website.Areas.Manage
{
    public class ManageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Manage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Manage_default",
                "Manage/{controller}/{action}/{id}",
                new {controller="Information", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}