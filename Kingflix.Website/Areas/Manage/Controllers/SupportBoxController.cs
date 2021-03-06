using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kingflix.Website.Areas.Manage.Controllers
{
    [Authorize]
    public class SupportBoxController : Controller
    {
        private readonly ISupportService _supportService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        public SupportBoxController
            (
                ISupportService supportService,
                IUserService userService,
                IProductService productService
            )
        {
            _supportService = supportService;
            _userService = userService;
            _productService = productService;
        }
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            return View(_supportService.GetSupportList(a => a.UserId == userId).ToList());
        }

        public ActionResult CreateSupport()
        {
            var userId = User.Identity.GetUserId();
            var profileOfUser =_productService.GetProfileList().Where(a => a.DateEnd.Date >= DateTime.Today && a.UserId == userId).ToList();
            var model = new Support()
            {
                UserId = userId,
                ProductList = profileOfUser.GroupBy(a => a.ProductId).Select(a => new Product() { ProductId = a.Key }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSupport([Bind(Include = "Title,Content,ProductId")] Support support, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                support.Status = SupportStatus.Pending;
                support.DateCreate = DateTime.Now;
                support.UserId = User.Identity.GetUserId();
                if (ImageUpload != null)
                {
                    int length = ImageUpload.ContentLength;
                    byte[] image = new byte[length];
                    ImageUpload.InputStream.Read(image, 0, length);
                    support.Image = image;
                }

                _supportService.CreateSupport(support);
                return RedirectToAction(nameof(Index));
            }

            return View(support);
        }

        // GET: Support/Edit/5
        public ActionResult EditSupport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            Support support = _supportService.GetSupportById(id);
            if (support == null)
            {
                return HttpNotFound();
            }
            if (support.UserId != userId)
                return HttpNotFound();
            return View(support);
        }

        [HttpPost]
        public ActionResult DeleteSupport(int id)
        {
            var result = new ResultViewModel();
            try
            {
                Support item = _supportService.GetSupportById(id);
                item.Status = SupportStatus.Cancel;
                _supportService.UpdateSupport(item);
                result.status = "success";
                result.message = "Thành công! Đã xóa danh mục bài viết.";
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