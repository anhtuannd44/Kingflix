using Kingflix.Domain.DomainModel;
using Kingflix.Models.ViewModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel.IdentityModel;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmailController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IRepository<AppUser> _userRepository;
        private readonly IRepository<EmailHistory> _emailHistoryRepository;
        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IRepository<Profile> _profileRepository;
        public EmailController(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IRepository<AppUser> userRepository,
            IRepository<EmailHistory> emailHistoryRepository,
            IRepository<EmailTemplate> emailTemplateRepository,
            IRepository<Profile> profileRepository
            )
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userRepository = userRepository;
            _emailHistoryRepository = emailHistoryRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _profileRepository = profileRepository;
        }

        public ActionResult Index()
        {
            var userList = _userRepository.GetAll();
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
            var userList = _userRepository.GetAll();
            var emailList = new string[userList.Count()];
            switch (Type)
            {
                case EmailType.PreExpired:
                    {
                        emailList = _profileRepository.GetAll().Where(a => a.DateEnd.Date <= DateTime.Today.AddDays(7).Date && !string.IsNullOrEmpty(a.UserInformation.Email)).Select(a => a.UserInformation.Email).ToArray();
                        break;
                    }
                case EmailType.Exprired:
                    {
                        emailList = _profileRepository.GetAll().Where(a => a.DateEnd.Date < DateTime.Today.Date && !string.IsNullOrEmpty(a.UserInformation.Email)).Select(a => a.UserInformation.Email).ToArray();
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
            var model = _emailTemplateRepository.GetAll();
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
                    var user = _userRepository.Get(a => a.Email == item).FirstOrDefault();
                    var emailHistoryItem = new EmailHistory();
                    switch (email.Type)
                    {
                        case EmailType.PreExpired:
                            var profilePreExpried = _profileRepository.GetAll().Where(a => a.UserId == user.Id && a.DateEnd.AddDays(-7).Date <= DateTime.Today.Date && a.DateEnd.Date >= DateTime.Today).ToList();
                            emailHistoryItem = _emailService.SendExpired(item, user.FullName, profilePreExpried, EmailType.PreExpired);
                            break;
                        case EmailType.Exprired:
                            var profileExpried = _profileRepository.GetAll().Where(a => a.UserId == user.Id && a.DateEnd.Date < DateTime.Today).ToList();
                            emailHistoryItem = _emailService.SendExpired(item, user.FullName, profileExpried, EmailType.PreExpired);
                            break;
                        default:
                            var emailTemplate = _emailTemplateRepository.GetById(email.EmailTemplateId);
                            emailHistoryItem = _emailService.SendEmailCustom(item, emailTemplate);
                            break;

                    }
                    _ = emailHistoryItem.Status == EmailStatus.Success ? successCount++ : errorCount++;
                    _emailHistoryRepository.Create(emailHistoryItem);

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
            var model = _emailTemplateRepository.GetAll();
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
                _emailTemplateRepository.Create(email);
                _unitOfWork.SaveChanges();
                return RedirectToAction("EmailTemplate");
            }
            return View(email);
        }

        public ActionResult EditEmailTemplate(int? id)
        {
            if (id == null)
                return HttpNotFound();
            var model = _emailTemplateRepository.GetById(id);
            if (model == null)
                return HttpNotFound();
            return View(model);
        }


        [HttpPost]
        public ActionResult EditEmailTemplate([Bind(Include = "Id,Title,Content")] EmailTemplate email)
        {
            if (ModelState.IsValid)
            {
                _emailTemplateRepository.Update(email);
                _unitOfWork.SaveChanges();
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
                var item = _emailTemplateRepository.GetById(id);
                _emailTemplateRepository.Delete(item);
                _unitOfWork.SaveChanges();
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