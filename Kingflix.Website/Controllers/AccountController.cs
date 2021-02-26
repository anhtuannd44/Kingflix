using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Data.Identity;
using Kingflix.Services.Interfaces;
using Kingflix.Services.Services;
using Kingflix.Website.CustomFilters;
using Kingflix.Website.Models;
using Kingflix.Website.Models.Custom;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;

namespace Kingflix.Website.Controllers
{
    public class AccountController : BaseExtendedController
    {
        //private readonly IAuthenticationManager _authManager;
        private readonly IEmailService _emailService;
        private readonly IRepository<EmailHistory> _emailHistoryRepository;

        public AccountController(
            //IAppUserManager userManager,
            //IAuthenticationManager authManager,
            IUnitOfWork unitOfWork,
            AppSignInManager signInManager,
            IEmailService emailService,
            IRepository<EmailHistory> emailHistoryRepository,
            IMapper mapper)
            : base(unitOfWork, /*userManager*/ signInManager, mapper)
        {
            //_authManager = authManager;
            _emailService = emailService;
            _emailHistoryRepository = emailHistoryRepository;
            _logger = LogManager.GetCurrentClassLogger();
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("_Error", new string[] { GetErrorMessage.NoAccess });
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Login(UserLoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        return Redirect(returnUrl ?? "/");
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Đăng nhập thất bại! Vui lòng thử lại");
                        return View(model);
                }
            }
            return View(model);
        }

        // GET: /Account/Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            _signInManager.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Create
        [AllowAnonymous]
        [ReturnUrl]
        public ActionResult Register(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Create
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Register(UserCreateViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect(UrlManager.PopUrl());
            }

            if (ModelState.IsValid)
            {
                AppUser user = _mapper.Map<AppUser>(model);
                user.DateCreated = DateTime.Now;
                user.TimeStep2 = DateTime.Now;
                user.TimeLastLogin = DateTime.Now;
                user.EmailConfirmed = true;

                var result = await _signInManager.UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var loginResult = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
                    if (loginResult == SignInStatus.Success)
                        return Redirect(returnUrl ?? "/");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }

            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var result = new ResultViewModel();
            if (ModelState.IsValid)
            {
                var user = await _signInManager.UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    result.status = "error";
                    result.message = "Email này không tồn tại, vui lòng thử lại!";
                }
                else
                {
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await _signInManager.UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, email = user.Email, code }, protocol: Request.Url.Scheme);

                    var sendEmail = _emailService.SendEmailForgotPassword(model.Email, user.FullName ?? user.Email, callbackUrl);
                    _emailHistoryRepository.Create(sendEmail);
                    _unitOfWork.SaveChanges();
                    if (sendEmail.Status == EmailStatus.Success)
                    {
                        result.status = "success";
                        result.message = "Thành công! Một liên kết đã được gửi tới Email của bạn. Vui lòng kiểm tra Email";
                    }
                }
            }
            else
            {
                result.status = "error";
                result.message = "Có lỗi khi nhập dữ liệu";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string email, string code)
        {
            ViewBag.Email = email;
            return code == null ? View("Error") : string.IsNullOrEmpty(userId) ? View("Error") : string.IsNullOrEmpty(email) ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var result = new ResultViewModel();
            if (!ModelState.IsValid)
            {
                result.status = "error";
                result.message = "Lỗi nhập dữ liệu. Vui lòng thử lại";
            }
            else
            {
                var user = await _signInManager.UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    result.status = "error";
                    result.message = "Không tìm thấy Email thích hợp, vui lòng thử lại!";
                }
                else
                {
                    var resetPasswordResult = await _signInManager.UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                    if (resetPasswordResult.Succeeded)
                    {
                        result.status = "success";
                        result.message = "Thành công! Mật khẩu của bạn đã được đặt lại. Đăng nhập để tiếp tục";
                    }
                    else
                    {
                        result.status = "error";
                        result.message = "Có lỗi khi đặt lại mật khẩu, vui lòng thử lại";
                    }
                }
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [NonAction]
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            ExternalLoginInfo loginInfo = await _signInManager.AuthenticationManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _signInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return Redirect(returnUrl ?? "/");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    var currentUser = await _signInManager.UserManager.FindByEmailAsync(loginInfo.Email);
                    if (currentUser != null)
                    {
                        var addLoginResult = await _signInManager.UserManager.AddLoginAsync(currentUser.Id, loginInfo.Login);
                        if (addLoginResult.Succeeded)
                        {
                            await _signInManager.SignInAsync(currentUser, isPersistent: false, rememberBrowser: false);
                            return Redirect(returnUrl ?? "/");
                        }
                    }
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }

        }
        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("UserInformation", "Manage");
            }

            if (ModelState.IsValid)
            {
                var loginInfo = await _signInManager.AuthenticationManager.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new AppUser
                {
                    Email = loginInfo.Email,
                    UserName = loginInfo.Email,
                    DateCreated = DateTime.Now,
                    TimeStep2 = DateTime.Now,
                    TimeLastLogin = DateTime.Now,
                    EmailConfirmed = true,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _signInManager.UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _signInManager.UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return Redirect(returnUrl ?? "/");
                    }
                }
                AddErrorsFromResult(result);
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }


        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";
        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}