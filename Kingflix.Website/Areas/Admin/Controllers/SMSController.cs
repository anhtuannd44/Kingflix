using Kingflix.Domain.DomainModel;
using Kingflix.Models.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel.IdentityModel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class SMSController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISMSAPIService _smsAPIService;
        private readonly IRepository<AppUser> _userRepository;
        private readonly IRepository<SMSHistory> _smsHistoryRepository;
        private readonly IRepository<SMSTemplate> _smsTemplateRepository;
        private readonly IRepository<Profile> _profileRepository;
        public SMSController(
            IUnitOfWork unitOfWork,
            ISMSAPIService smsAPIService,
            IRepository<AppUser> userRepository,
            IRepository<SMSHistory> smsHistoryRepository,
            IRepository<SMSTemplate> smsTemplateRepository,
            IRepository<Profile> profileRepository
            )
        {
            _unitOfWork = unitOfWork;
            _smsAPIService = smsAPIService;
            _userRepository = userRepository;
            _smsHistoryRepository = smsHistoryRepository;
            _smsTemplateRepository = smsTemplateRepository;
            _profileRepository = profileRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult GetUserPhone()
        {
            var model = _userRepository.Get(a=>string.IsNullOrEmpty(a.PhoneNumber)).ToList();
            return PartialView("_SMSListPartial", model);
        }

        public ActionResult GetSMSTemplateList()
        {
            var model = _smsTemplateRepository.GetAll();
            return PartialView("_SMSTemplateListPartial", model);
        }

        [HttpPost]
        public JsonResult SendSMS(SMSViewModel sms)
        {
            var result = new ResultViewModel();
            var errorCount = 0;
            var successCount = 0;
            var smsTemplate = _smsTemplateRepository.GetById(sms.SMSTemplateId);
            foreach (var item in sms.UserId)
            {
                try
                {
                    var user = _userRepository.GetById(item);
                    JObject sendSMS = _smsAPIService.SendSMS(user.PhoneNumber, smsTemplate.Content);
                    if ((int)sendSMS["CodeResult"] == 100)
                    {
                        var smsHistoryItem = new SMSHistory()
                        {
                            PhoneNumber = user.PhoneNumber,
                            Content = smsTemplate.Content,
                            Title = smsTemplate.Title,
                            TimeSend = DateTime.Now,
                            UserId = user.Id
                        };
                        _smsHistoryRepository.Create(smsHistoryItem);
                        _unitOfWork.SaveChanges();
                        successCount++;
                    }
                    else
                    {
                        errorCount++;
                    }
                }
                catch
                {
                    errorCount++;
                }
            }
            result.status = "success";
            result.message = "Thành công! Đã gửi thành công " + successCount + " SMS và " + errorCount + " lỗi!";
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public ActionResult SMSTemplate()
        {
            var model = _smsTemplateRepository.GetAll();
            return View(model);
        }
        public ActionResult CreateSMSTemplate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateSMSTemplate([Bind(Include = "Title,Content")] SMSTemplate sms)
        {
            if (ModelState.IsValid)
            {
                _smsTemplateRepository.Create(sms);
                _unitOfWork.SaveChanges();
                return RedirectToAction("SMSTemplate");
            }
            return View(sms);
        }

        public ActionResult EditSMSTemplate(int? id)
        {
            if (id == null)
                return HttpNotFound();
            var model = _smsTemplateRepository.GetById(id);
            if (model == null)
                return HttpNotFound();
            return View(model);
        }


        [HttpPost]
        public ActionResult EditSMSTemplate([Bind(Include = "Id,Title,Content")] SMSTemplate sms)
        {
            if (ModelState.IsValid)
            {
                _smsTemplateRepository.Update(sms);
                _unitOfWork.SaveChanges();
                return RedirectToAction("SMSTemplate");
            }
            return View(sms);
        }

        [HttpPost]
        public ActionResult DeleteSMSTemplate(int id)
        {
            var result = new ResultViewModel();
            try
            {
                var item = _smsTemplateRepository.GetById(id);
                _smsTemplateRepository.Delete(item);
                _unitOfWork.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Đã xóa mẫu sms";
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