using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Data;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Route("{action}")]
    public class GoogleFormController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();
        public ActionResult Index()
        {
            var model = db.GoogleForm.ToList();
            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Title,Url,Status")] GoogleForm form)
        {
            if (ModelState.IsValid)
            {
                form.DateCreated = DateTime.Now;
                db.GoogleForm.Add(form);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(form);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return HttpNotFound();
            var model = db.GoogleForm.Find(id);
            if (model == null)
                return HttpNotFound();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,DateCreated,Title,Url,Status")] GoogleForm form)
        {
            if (ModelState.IsValid)
            {
                var item = db.GoogleForm.Find(form.Id);
                item.Title = form.Title;
                item.Url = form.Url;
                item.Status = form.Status;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(form);
        }

        [HttpPost]
        public ActionResult MoveToTrash(int? id)
        {
            var result = new ResultViewModel();
            try
            {
                var item = db.GoogleForm.Find(id);
                item.Status = Status.Trash;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Đã đưa form vào thùng rác!";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi trong quá trình đưa vào thùng rác. Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult Delete(int? id)
        {
            var result = new ResultViewModel();
            try
            {
                var item = db.GoogleForm.Find(id);
                db.GoogleForm.Remove(item);
                db.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Đã xóa Form khảo sát!";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi trong quá trình xóa. Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}