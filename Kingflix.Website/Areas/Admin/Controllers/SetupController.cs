using Kingflix.Domain.DomainModel;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;
using System.Web.Mvc;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class SetupController : Controller
    {
        private readonly ISettingService _settingService;
        public SetupController(ISettingService settingService)
        {
            _settingService = settingService;
        }
        public ActionResult Policy()
        {
            var model = _settingService.GetPolicy();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdatePolicy([Bind(Include = "SettingId,Title,Content,Status")] Setting policy)
        {
            if (ModelState.IsValid)
            {
                _settingService.UpdatePolicy(policy);
                return RedirectToAction("Policy");
            }
            return View(policy);
        }

        [HttpPost]
        public ActionResult Downpage(bool isDownPage)
        {
            var result = new ResultViewModel();
            try
            {
                var downpage = _settingService.GetDownpage();
                downpage.IsDownPage = isDownPage;
                _settingService.UpdateDownpage(downpage);
                result.status = "success";
                if (isDownPage)
                    result.message = "Thành công! Trang của đã chuyển thành bảo trì";
                else
                    result.message = "Thành công! Trang của đã hoạt động trở lại";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public string GetDownPage()
        {
            var check = _settingService.GetDownpage().IsDownPage;
            if (check.HasValue)
            {
                if (check.Value)
                    return "True";
                return "False";
            }
            return "False";
        }
        public ActionResult TopText()
        {
            var model = _settingService.GetToptext();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdateTopText(string Content)
        {
            _settingService.UpdateToptext(Content);
            return RedirectToAction("TopText");
        }

        public ActionResult BaoTri()
        {
            var model = _settingService.GetBaoTriPage();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult UpdateBaoTri([Bind(Include = "SettingId,Title,Content")] Setting baotri)
        {
            if (ModelState.IsValid)
            {
                _settingService.UpdateBaoTriPage(baotri);
                return RedirectToAction("BaoTri");
            }
            return View(baotri);
        }
        public ActionResult RefundSetting()
        {
            var model = _settingService.GetRefundItem();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateRefundSetting(double? Value)
        {

            var item = _settingService.GetRefundItem();
            item.Value = Value;
            _settingService.UpdateRefundSetting(item);
            return RedirectToAction("RefundSetting");
        }
    }
}