using System.Net;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ISettingService _settingService;
        public QuestionController(
            ISettingService settingService
            )
        {
            _settingService = settingService;
        }
        public ActionResult Index()
        {
            var model = _settingService.GetQuestionList();
            return View(model);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Question question = _settingService.GetQuestionById(id);
            if (question == null)
                return HttpNotFound();
            return View(question);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Title,Content")] Question question)
        {
            if (ModelState.IsValid)
            {
                _settingService.CreateQuestion(question);
                return RedirectToAction("Index");
            }

            return View(question);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Question question = _settingService.GetQuestionById(id);
            if (question == null)
                return HttpNotFound();
            return View(question);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content")] Question question)
        {
            if (ModelState.IsValid)
            {
                _settingService.UpdateQuestion(question);
                return RedirectToAction("Index");
            }
            return View(question);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = new ResultViewModel();
            try
            {
                _settingService.DeleteQuestion(id);
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
