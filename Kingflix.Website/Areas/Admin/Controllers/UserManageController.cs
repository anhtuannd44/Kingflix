using Kingflix.Domain.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kingflix.Utilities;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.ViewModel;
using Kingflix.Models.ViewModel;
using Kingflix.Domain.Abstract;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManageController : Controller
    {
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly IEmailService _emailService;
        private readonly ISupportService _supportService;
        private readonly IRepository<EmailHistory> _emailHistoryRepository;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        public UserManageController(
            IUserService userService,
            IProductService productService,
            IEmailService emailService,
            ISupportService supportService,
            IRepository<EmailHistory> emailHistoryRepository,
            IRepository<EmailTemplate> emailTemplateRepository
            )
        {
            _userService = userService;
            _productService = productService;
            _emailService = emailService;
            _supportService = supportService;
            _emailHistoryRepository = emailHistoryRepository;
            _emailTemplateRepository = emailTemplateRepository;
        }
        public ActionResult Index(string categoryId = Const.NETFLIX0, Cycle? cycle = null, string targetAccount = null)
        {
            if (targetAccount != null)
            {
                var item = _productService.GetProductById(targetAccount);
                if (item == null)
                    return HttpNotFound();
                categoryId = item.CategoryId;
            }
            var model = _productService.GetProductList(a => a.CategoryId == categoryId).ToList();
            if (categoryId == Const.NETFLIX2)
                model = model.Where(a => a.Cycle == cycle).ToList();
            model.ForEach(a => a.Parent = _productService.GetProductById(a.ParentId));
            ViewBag.TargetAccount = targetAccount;
            ViewBag.CategoryId = categoryId;
            return View(model);
        }
        public ActionResult PreExtend()
        {
            var model = _productService.GetProductList(b => b.Profiles.Where(a => !string.IsNullOrEmpty(a.UserId) && a.DateEnd <= DateTime.Today).Count() != 0).ToList();
            return View(model);
        }
        public ActionResult DeleteUser()
        {
            var model = _productService.GetProfileList(a => !string.IsNullOrEmpty(a.UserId) && a.DateEnd < DateTime.Today).OrderBy(a => a.ProductId).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteUser(int[] profileList)
        {
            var result = new ResultViewModel();
            try
            {
                _productService.RemoveUserFromProfile(profileList);
                result.status = "success";
                result.message = "Đã xóa người dùng hết hạn khỏi tài khoản thành công!";
            }
            catch (Exception ex)
            {
                result.status = "error";
                result.message = "Có lỗi xảy ra, vui lòng thử lại!";
            }
            return Json(result);
        }
        public ActionResult DeleteProduct()
        {
            var model = _productService.GetProductList().Where(a => a.DateEnd < DateTime.Today).ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteProduct(string[] productList) //Xóa các tài khoản hết hạn
        {
            var result = new ResultViewModel();
            try
            {
                result = _productService.DeleteProductList(productList);
            }
            catch (Exception ex)
            {
                result.status = "error";
                result.message = "Có lỗi xảy ra, vui lòng thử lại!";
            }
            return Json(result);
        }
        [HttpPost]
        public ActionResult GetProfileList(string productId)
        {
            var profileList = _productService.GetProfileList(a => a.ProductId == productId).ToList();
            var userList = _userService.GetUserList();
            foreach (var item in profileList)
            {
                item.IsError = _supportService.GetSupportList(a => item.ProductId == item.ProductId).Count() > 0;
            }
            var product = _productService.GetProductById(productId);
            if (!string.IsNullOrEmpty(product.ParentId))
            {
                var changeAccount = _productService.GetProductById(product.ParentId);
                if (product.CategoryId == Const.NETFLIX3)
                    profileList.FirstOrDefault().ProfileId = changeAccount.Profiles.FirstOrDefault().ProfileId;
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        profileList[i].ProfileId = changeAccount.Profiles.ToList()[i].ProfileId;
                        profileList[i].Name = changeAccount.Profiles.ToList()[i].Name;
                        profileList[i].Pincode = changeAccount.Profiles.ToList()[i].Pincode;
                    }
                }
            }
            var model = new UserManageViewModel()
            {
                Profile = profileList,
                UserList = userList.ToList()
            };
            ViewData["CategoryId"] = _productService.GetAllCategories().ToList();
            return PartialView("_ProfileListPartial", model);
        }

        [HttpPost]
        public ActionResult EditProfileList(List<Profile> ProfileList)
        {
            var result = new ResultViewModel();
            try
            {
                _productService.EditProfileList(ProfileList);
                result.status = "success";
                result.message = "Thành công! Dữ liệu đã được cập nhật";
            }
            catch (Exception ex)
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng kiểm tra và thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public ActionResult CreateProfile(string productId)
        {
            var result = new ResultViewModel();
            try
            {
                _productService.CreateProfile(productId);
                result.status = "success";
                result.message = "Thành công! Đã tạo thêm Profile";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi trong quá trình tạo Profile";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult LoadNewProfile(string productId)
        {
            var profile = _productService.GetProfileList(a => a.ProductId == productId);
            ViewBag.I = profile.Count();
            var model = profile.OrderByDescending(a => a.DateCreated).FirstOrDefault();
            return PartialView("_NewProfilePartial", model);
        }

        public PartialViewResult GetProfileNull(string categoryId, string currentProduct)
        {
            var model = _productService.GetProductList(a => a.CategoryId == categoryId && a.Categories.TypeOfAccount == TypeOfAccount.KingflixAccount && a.ProductId != currentProduct && a.Profiles.Where(b => string.IsNullOrEmpty(b.UserId)).Count() > 0).ToList();
            return PartialView("_ProfileNullListPartial", model);
        }

        [HttpPost]
        public ActionResult RemoveProfile(int profileId)
        {
            var result = new ResultViewModel();
            try
            {
                _productService.DeleteProfile(profileId);
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xảy ra, vui lòng thử lại!";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult ChangeProfile(int currentProfile, string product)
        {
            var result = new ResultViewModel();
            try
            {
                _productService.ChangeProfile(currentProfile, product);
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xả ra, vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult ResetProfile(int profileId)
        {
            var result = new ResultViewModel();
            try
            {
                _productService.ResetProfile(profileId);
                result.status = "success";
                result.message = "Thành công! Dữ liệu đã được cập nhật";
            }
            catch (Exception ex)
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng kiểm tra và thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult EditProduct(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                return HttpNotFound();
            Product product = _productService.GetProductById(productId);
            if (product == null)
                return HttpNotFound();

            ViewBag.CategoryId = new SelectList(_productService.GetAllCategories().Where(a => a.TypeOfAccount == TypeOfAccount.KingflixAccount), "CategoryId", "Name", product.CategoryId);
            var changeProduct = _productService.GetProductList().Where(a => a.CategoryId == "CHANGEACCOUNT" && a.DateEnd.Date >= DateTime.Today).ToList();
            List<Product> changeProductSelect = new List<Product>();
            foreach (var item in changeProduct)
            {
                var check = _productService.GetProductList(a => a.ParentId == item.ProductId).Count();
                if (check == 0)
                    changeProductSelect.Add(item);
            }
            ViewBag.ChangeProduct = changeProductSelect;
            return PartialView("_EditProductPartial", product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EditProductConfirm([Bind(Include = "ProductId,Password,Pincode,DateEnd,DateCreated,Status,ReplaceProduct,ProfileList,ParentId,CategoryId")] Product product, string newPassword, bool changePassRequired, string currentParentId)
        {
            var result = new ResultViewModel();
            try
            {
                if (product.Status == ProductStatus.Replace && string.IsNullOrEmpty(product.ParentId))
                {
                    result.status = "error";
                    result.message = "Thất bại! Bạn chưa chọn tài khoản thay thế";
                }
                else
                    result = _productService.EditProductConfirm(product, newPassword, changePassRequired, currentParentId);
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xảy ra, vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult SendEmail(string email)
        {
            ViewBag.Email = email;
            ViewData["EmailTemplate"] = _emailTemplateRepository.GetAll();
            return PartialView("_EmailPartial");
        }

        [HttpPost]
        public ActionResult EmailHistory(string email)
        {
            var model = _emailHistoryRepository.Get(a => a.Email == email).OrderByDescending(a => a.TimeSend);
            return PartialView("_EmailHistoryPartial", model);
        }

        #region
        //public bool RemoveAllDataNotTrue()
        //{
        //    var profile = db.Profile.ToList();
        //    var order = db.Order.Where(a => string.IsNullOrEmpty(a.UserId));
        //    var userList = appDb.Users.Select(a => a.Id).ToArray();
        //    foreach (var i in profile.Where(a => !string.IsNullOrEmpty(a.UserId)).ToList())
        //    {
        //        var check = false;
        //        foreach (var j in userList)
        //        {
        //            if (j == i.UserId)
        //            {
        //                check = true;
        //                break;
        //            }
        //        }
        //        if (!check)
        //        {
        //            i.UserId = null;
        //            db.Entry(i).State = EntityState.Modified;
        //        }
        //    }
        //    foreach (var i in profile)
        //    {
        //        if (i.ProductId == null)
        //            db.Profile.Remove(i);
        //    }
        //    foreach (var i in order)
        //    {
        //        var check = false;
        //        foreach (var j in userList)
        //        {
        //            if (j == i.UserId)
        //            {
        //                check = true;
        //                break;
        //            }
        //        }
        //        if (!check)
        //        {
        //            db.Order.Remove(i);
        //        }
        //    }
        //    db.SaveChanges();
        //    return true;
        //}
        #endregion
    }
}