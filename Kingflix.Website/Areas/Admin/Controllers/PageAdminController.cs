using System.Net;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class PageAdminController : Controller
    {
        private readonly IPageService _pageService;
        public PageAdminController(IPageService pageService)
        {
            _pageService = pageService;
        }
        public ActionResult Index()
        {
            var model = _pageService.GetPageList();
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Title,Url,Description,MetaDescription,MetaKeyword,PageContent,Status")] Page page)
        {
            if (ModelState.IsValid)
            {
                _pageService.CreatePage(page);
                return RedirectToAction("Index");
            }
            return View(page);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Page page = _pageService.GetPageById(id);
            if (page == null)
                return HttpNotFound();
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "PageId,Title,Url,Description,MetaDescription,MetaKeyword,PageContent,DateCreated,Status")] Page page)
        {
            if (ModelState.IsValid)
            {
                _pageService.UpdatePage(page);
                return RedirectToAction("Index");
            }
            return View(page);
        }

        // GET: Admin/PageAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            var result = new ResultViewModel();
            try
            {
                _pageService.DeletePage(id);
                result.status = "success";
                result.message = "Thành công! Đã xóa trang hoàn toàn";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}
