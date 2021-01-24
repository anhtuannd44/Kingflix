using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Services.Data.Identity.Abstraction;
using Kingflix.Services.Services;
using Kingflix.Website.CustomFilters;
using Kingflix.Website.Models;
using Kingflix.Website.Models.Custom;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NLog;

namespace Kingflix.Website.Controllers
{
    public class AccountController : BaseExtendedController
    {
        private readonly IAuthenticationManager _authManager;
        private readonly IMailingService _mailingRepository;

        public AccountController(
            IAppUserManager userManager,
            IAuthenticationManager authManager,
            IMailingService mailingRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(unitOfWork, userManager, mapper)
        {
            _authManager = authManager;
            _mailingRepository = mailingRepository;
            _logger = LogManager.GetCurrentClassLogger();
        }

        // GET: /Account/Login
        [ReturnUrl]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("_Error", new string[] { GetErrorMessage.NoAccess });
            }

            return View();
        }

        // POST: /Account/Login
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Login(UserLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindAsync(model.Email, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", GetErrorMessage.InvalidNameOrPassword);
                }
                else
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user.Id))
                    {
                        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        //_mailingRepository.ActivationMail(user.Email, callbackUrl);
                        return View("_Error", new string[] { "You must have a confirmed email to log on. Check your email for activation link." });
                    }

                    ClaimsIdentity ident = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    _authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    _authManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false,
                    }, ident);

                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        // GET: /Account/Logout
        [Authorize]
        public ActionResult Logout()
        {
            _authManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/Create
        [AllowAnonymous]
        [ReturnUrl]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("Index","Home");
            }

            return View();
        }

        // POST: /Account/Create
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Register(UserCreateViewModel model)
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

                IdentityResult result = new IdentityResult();
                result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //string code = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    //string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    //_mailingRepository.WelcomeMail(user.Email);
                    //_mailingRepository.ActivationMail(user.Email, callbackUrl);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }

            return View(model);
        }

        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword?email
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", GetErrorMessage.NullUser);
                return View();
            }

            if (!string.IsNullOrWhiteSpace(email) && email.Contains("@"))
            {
                string code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                string callbackUrl = Url.Action("ResetPassword", "Account", new { code = code }, protocol: Request.Url.Scheme);
                //_mailingRepository.ResetPasswordMail(user.Email, callbackUrl);
                return View("ForgotPasswordConfirmation");
            }
            ModelState.AddModelError("", GetErrorMessage.NoEmail);
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return string.IsNullOrWhiteSpace(code) ? View("_Error", new string[] { "Something went wrong." }) : View();
        }

        [NonAction]
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}