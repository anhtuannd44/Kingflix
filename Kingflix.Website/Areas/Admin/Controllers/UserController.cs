using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        public UserController(
            IUserService userService,
            IProductService productService,
            IOrderService orderService)
        {
            _userService = userService;
            _productService = productService;
            _orderService = orderService;
        }
        public ActionResult Index()
        {
            var model = _userService.GetUserList();
            return View(model);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppUser AppUser = _userService.GetUserById(id);
            if (AppUser == null)
            {
                return HttpNotFound();
            }
            return View(AppUser);
        }
        public ActionResult GetUserList(string userId)
        {
            var model = _userService.GetUserList();
            if (model.Count() != 0 && !string.IsNullOrEmpty(userId))
                foreach (var item in model)
                {
                    if (item.Id == userId)
                        item.IsSelected = true;
                }
            return PartialView("_UserListPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FullName,Address,Gentle,Birthday,PhoneNumber,Email")] AppUser AppUser)
        {
            if (ModelState.IsValid)
            {
                _userService.UpdateUser(AppUser);
                return RedirectToAction("Index");
            }
            return View(AppUser);
        }

        public ActionResult GetProfile(string userId, TypeOfCategory type)
        {
            var model = _productService.GetProfileList(a => a.Products.Categories.Type == type && a.UserId == userId).ToList();
            ViewBag.Type = type.GetEnumDisplayName();
            return PartialView("_ProfilePartial", model);
        }
        public ActionResult GetOrderHistory(string userId)
        {
            var model = _orderService.GetOrderList(a => a.UserId == userId && a.Type == OrderType.Order).ToList();
            return PartialView("_OrderHistoryPartial", model);
        }

        // POST: Admin/User/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            var result = new ResultViewModel();
            try
            {
               
                result.status = "success";
                result.message= "Thành công! Thành viên đã được xóa.";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Không thể xóa.";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}
