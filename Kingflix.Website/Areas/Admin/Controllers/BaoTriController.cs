using System.Web.Mvc;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BaoTriController : Controller
    {
        private readonly ISettingService _settingService;
        public BaoTriController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            var model = _settingService.GetBaoTriPage();
            return View("~/Areas/Admin/Views/BaoTri/Index.cshtml", model);
        }
    }
}