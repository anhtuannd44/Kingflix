using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Data.Identity;
using Kingflix.Services.Interfaces;
using Kingflix.Services.Services;
using Kingflix.Website.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Kingflix.Website.Areas.Manage.Controllers
{
    [Authorize]
    public class InformationController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISettingService _settingService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly AppSignInManager _signInManager;
        public InformationController
            (
                IUserService userService,
                ISettingService settingService,
                IProductService productService,
                IOrderService orderService,
                AppSignInManager signInManager
            )
        {
            _userService = userService;
            _settingService = settingService;
            _productService = productService;
            _orderService = orderService;
            _signInManager = signInManager;
        }
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = _userService.GetUserById(userId);
            var model = new UserViewModel()
            {
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
                Gentle = user.Gentle,
                Address = user.Address,
                FeedbackStar = user.FeedbackStar,
                FeedbackContent = user.FeedbackContent,
                Referral = user.ReferralCode,
                Avatar = user.Avatar
            };
            return View(model);
        }
        public ActionResult GetSetting()
        {
            var model = _settingService.GetPolicy();
            return PartialView("_PolicyPartial", model);
        }
        public PartialViewResult GetProfile(TypeOfCategory Type)
        {
            var userId = User.Identity.GetUserId();
            var profile = _productService.GetProfileList(a => a.UserId == userId && a.Products.Categories.Type == Type).ToList();
            ViewBag.IsPending = "False";
            var order = _orderService.GetOrderList(a => a.UserId == userId && a.Status == OrderStatus.WaitingForPay).Count();
            if (order > 0)
                ViewBag.IsPending = "True";
            if (Type == TypeOfCategory.Netflix)
                return PartialView("_ProfileNetflixPartial", profile);

            else
                return PartialView("_ProfileSpotifyPartial", profile);
        }

        [HttpPost]
        public string GetAvatar()
        {
            string base64 = "", imgSrc = "";
            var userId = User.Identity.GetUserId();
            var avatar = _userService.GetUserById(userId).Avatar;
            if (avatar != null)
            {
                base64 = Convert.ToBase64String(avatar);
                imgSrc = string.Format("data:image/gif;base64,{0}", base64);
            }
            else
            {
                imgSrc = "../../Content/Image/No_Picture.jpg";
            }
            return imgSrc;
        }
        public ActionResult UpdateUserInformation(UserViewModel user, HttpPostedFileBase avatars)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var item = _userService.GetUserById(userId);
                item.FullName = user.FullName;
                item.PhoneNumber = user.PhoneNumber;
                item.Birthday = user.Birthday;
                item.Gentle = user.Gentle;
                item.Address = user.Address;
                item.DateModified = DateTime.Now;

                if (avatars != null)
                {
                    int length = avatars.ContentLength;
                    byte[] image = new byte[length];
                    avatars.InputStream.Read(image, 0, length);
                    item.Avatar = image;
                }
                _userService.UpdateUser(item);

            }
            catch
            {
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SendFeedback(double star, string content)
        {
            var result = new ResultViewModel();
            try
            {
                var userId = User.Identity.GetUserId();
                var item = _userService.GetUserById(userId);
                item.FeedbackStar = star;
                item.FeedbackContent = content;
                _userService.UpdateUser(item);
                result.status = "success";
                result.message = "Thành công! Nhận xét của bạn đã được ghi nhận";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng kiểm tra và thử lại.";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(UserChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", GetErrorMessage.PasswordsDontMatch);
                return View(model);
            }

            string userId = User.Identity.GetUserId();
            AppUser user = _userService.GetUserById(userId);
            if (user != null)
            {
                bool correctPass = await _signInManager.UserManager.CheckPasswordAsync(user, model.OldPassword);
                if (!correctPass)
                {
                    ModelState.AddModelError("", GetErrorMessage.PasswordNotValid);
                    return View(model);
                }

                IdentityResult validPass = await _signInManager.UserManager.PasswordValidator.ValidateAsync(model.NewPassword);
                if (validPass.Succeeded)
                {
                    user.PasswordHash = _signInManager.UserManager.PasswordHasher.HashPassword(model.NewPassword);

                    IdentityResult result = await _signInManager.UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        //_mailingRepository.PasswordChangedMail(user.Email);
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
                else
                {
                    AddErrorsFromResult(validPass);
                }
            }

            ModelState.AddModelError("", GetErrorMessage.NullUser);
            return Json(model, JsonRequestBehavior.DenyGet);
        }
        public string GetKingCoin()
        {
            var userId = User.Identity.GetUserId();
            var user = _userService.GetUserById(userId);
            return string.Format("{0:C0}", user.KinCoin);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        [NonAction]
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}