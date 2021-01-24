using Kingflix.Domain.DomainModel;
using Kingflix.Models.ViewModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.ViewModel;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        public EmailController(
            IEmailService emailService,
            IUserService userService,
            IProductService productService
            )
        {
            _emailService = emailService;
            _userService = userService;
            _productService = productService;
        }

        public ActionResult Index()
        {
            var userList = _userService.GetUserList();
            var model = new EmailViewModel
            {
                EmailList = new string[userList.Count()]
            };
            model.EmailList = userList.Select(a => a.Email).ToArray();
            return View(model);
        }

        [HttpPost]
        public PartialViewResult GetUserEmail(EmailType Type)
        {
            var userList = _userService.GetUserList();
            var emailList = new string[userList.Count()];
            switch (Type)
            {
                case EmailType.PreExpired:
                    {
                        emailList = _productService.GetProfileList().Where(a => a.DateEnd.Date <= DateTime.Today.AddDays(7).Date && !string.IsNullOrEmpty(a.UserInformation.Email)).Select(a => a.UserInformation.Email).ToArray();
                        break;
                    }
                case EmailType.Exprired:
                    {
                        emailList = _productService.GetProfileList().Where(a => a.DateEnd.Date < DateTime.Today.Date && !string.IsNullOrEmpty(a.UserInformation.Email)).Select(a => a.UserInformation.Email).ToArray();
                        break;
                    }
                default:
                    {
                        emailList = userList.Select(a => a.Email).ToArray();
                        break;
                    };
            }
            var model = new EmailViewModel()
            {
                Type = Type,
                EmailList = emailList
            };
            return PartialView("_EmailListPartial", model);
        }

        public ActionResult GetEmailTemplateList()
        {
            var model = _emailService.GetEmailTemplateList();
            return PartialView("_EmailTemplateListPartial", model);
        }

        [HttpPost]
        public JsonResult SendEmail(EmailViewModel email)
        {
            var result = new ResultViewModel();
            var errorCount = 0;
            var successCount = 0;
            foreach (var item in email.EmailList)
            {
                try
                {
                    var user = _userService.GetUserList(a => a.Email == item).FirstOrDefault();
                    switch (email.Type)
                    {
                        case EmailType.PreExpired:
                            {
                                try
                                {
                                    var profile = _productService.GetProfileList().Where(a => a.UserId == user.Id && a.DateEnd.AddDays(-7).Date <= DateTime.Today.Date && a.DateEnd.Date >= DateTime.Today).ToList();
                                    var send = _emailService.SendExpired(item, user.FullName, profile, EmailType.PreExpired);
                                    if (send)
                                    _emailService.AddEmailHistory(EmailTypeHistory.PreExpired, item);
                                    successCount++;
                                }
                                catch
                                {
                                    errorCount++;
                                }
                                break;
                            }
                        case EmailType.Exprired:
                            {
                                try
                                {
                                    var profile = _productService.GetProfileList().Where(a => a.UserId == user.Id && a.DateEnd.Date < DateTime.Today).ToList();
                                    var send = _emailService.SendExpired(item, user.FullName, profile, EmailType.PreExpired);
                                    if (send)
                                        _emailService.AddEmailHistory(EmailTypeHistory.PreExpired, item);
                                    _emailService.SendExpired(item, user.FullName, profile, EmailType.Exprired);
                                    successCount++;
                                }
                                catch
                                {
                                    errorCount++;
                                }
                                break;
                            }
                        default:
                            {
                                try
                                {
                                    var emailTemplate = _emailService.GetEmailtemplateById(email.EmailTemplateId);
                                    var send = _emailService.SendEmailCustom(item, emailTemplate);
                                    if (send)
                                    {
                                        _emailService.AddEmailHistory(EmailTypeHistory.Custom, item);
                                        successCount++;
                                    }
                                    else
                                        errorCount++;
                                }
                                catch
                                {
                                    errorCount++;
                                }
                                break;
                            }
                    }

                }
                catch
                {
                }
            }
            result.status = "success";
            result.message = "Thành công! Đã gửi thành công " + successCount + " Email và " + errorCount + " lỗi!";
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public ActionResult EmailTemplate()
        {
            var model = _emailService.GetEmailTemplateList();
            return View(model);
        }
        public ActionResult CreateEmailTemplate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateEmailTemplate([Bind(Include = "Title,Content")] EmailTemplate email)
        {
            if (ModelState.IsValid)
            {
                _emailService.CreateEmailTemplate(email);
                return RedirectToAction("EmailTemplate");
            }
            return View(email);
        }

        public ActionResult EditEmailTemplate(int? id)
        {
            if (id == null)
                return HttpNotFound();
            var model = _emailService.GetEmailtemplateById(id);
            if (model == null)
                return HttpNotFound();
            return View(model);
        }


        [HttpPost]
        public ActionResult EditEmailTemplate([Bind(Include = "Id,Title,Content")] EmailTemplate email)
        {
            if (ModelState.IsValid)
            {
                _emailService.UpdateEmailTemplate(email);
                return RedirectToAction("EmailTemplate");
            }
            return View(email);
        }

        [HttpPost]
        public ActionResult DeleteEmailTemplate(int id)
        {
            var result = new ResultViewModel();
            try
            {
                _emailService.DeleteEmailTemplate(id);
                result.status = "success";
                result.message = "Thành công! Đã xóa mẫu email";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi khi xóa dữ liệu";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

    }
}