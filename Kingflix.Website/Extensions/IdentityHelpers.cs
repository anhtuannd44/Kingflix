using System.Web.Mvc;
using Kingflix.Services.Data.Identity.Abstraction;

namespace Kingflix.Website.Extensions
{
    public static class IdentityHelpers
    {
        /// <summary>
        /// Get AppUser's name.
        /// </summary>
        /// <param name="html"></param>
        /// <param name="id">Search user by this id.</param>
        /// <returns></returns>
        public static MvcHtmlString GetUserName(this HtmlHelper html, string id)
        {
            IAppUserManager manager = DependencyResolver.Current.GetService<IAppUserManager>();
            string result = manager.FindById(id)?.UserName;
            return new MvcHtmlString(result);
        }
    }
}