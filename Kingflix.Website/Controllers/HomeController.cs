using System.Web.Mvc;
using AutoMapper;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Services.Interfaces;
using Kingflix.Website.Models.Custom;
using NLog;

namespace Kingflix.Website.Controllers
{
    public class HomeController : BaseController
    {

        private readonly ISettingService _settingService;

        public HomeController(
            ISettingService settingService,
            IMapper mapper)
            : base(mapper)
        {
            _settingService = settingService;
            _logger = LogManager.GetCurrentClassLogger();
        }

        // GET: /
        public ActionResult Index()
        {
            var model = _settingService.GetListHomepageSlider();
            return View(model);
        }
        public ActionResult GetQuestion()
        {
            var model = _settingService.GetQuestionList();
            return PartialView("_QuestionPartial", model);
        }
        public ActionResult GetSmallPictureHome()
        {
            var model = _settingService.GetSmallPicture();
            return PartialView("_SmallPictureHomePartial", model);
        }
        public ActionResult GetFooterSocial()
        {
            var model = _settingService.GetSociallist();
            return PartialView("_FooterSocialPartial", model);
        }
        public ActionResult GoBack()
        {
            UrlManager.IsReturning = true;
            return Redirect(UrlManager.PopUrl());
        }
    }
}