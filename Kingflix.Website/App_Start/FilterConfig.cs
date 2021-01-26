using Kingflix.Domain.DomainModel;
using Kingflix.Services.Data;
using System.Web;
using System.Web.Mvc;

namespace Kingflix.Website.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CheckForDownPage());
        }
        public sealed class CheckForDownPage : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var o = HttpContext.Current.Request.RawUrl.ToLower().Contains("admin");
                if (HttpContext.Current.Request.RawUrl.ToLower().Contains("bao-tri") || o)
                {
                    if (!isCheckDown() && HttpContext.Current.Request.RawUrl.ToLower().Contains("bao-tri"))
                    {
                        filterContext.HttpContext.Response.Clear();
                        filterContext.HttpContext.Response.Redirect("~/");
                        return;
                    }
                    return;
                }

                if (isCheckDown())
                {
                    filterContext.HttpContext.Response.Clear();
                    filterContext.HttpContext.Response.Redirect("~/bao-tri");
                    return;
                }

                base.OnActionExecuting(filterContext);
            }
        }
        public static bool isCheckDown()
        {
            using (var db = new AppDbContext())
            {
                var downPage = db.Setting.Find("BaoTri");
                if (downPage == null)
                {
                    db.Setting.Add(new Setting()
                    {
                        SettingId = "BaoTri",
                        Title = "BẢO TRÌ HỆ THỐNG!",
                        Content = "Xin lỗi quý khách vì sự bất tiện này, vui lòng quay lại sau ít phút.",
                        IsDownPage = false
                    });
                    db.SaveChanges();
                    return false;
                }
                else
                {
                    if (downPage.IsDownPage.HasValue)
                    {
                        return downPage.IsDownPage.Value;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}