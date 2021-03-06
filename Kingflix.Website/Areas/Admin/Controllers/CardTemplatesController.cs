using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Data;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CardTemplatesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/CardTemplates
        public ActionResult Index()
        {
            return View(db.CardTemplates.ToList());
        }

        // GET: Admin/CardTemplates/Create
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Content")] CardTemplate cardTemplate)
        {
            if (ModelState.IsValid)
            {
                db.CardTemplates.Add(cardTemplate);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cardTemplate);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardTemplate cardTemplate = db.CardTemplates.Find(id);
            if (cardTemplate == null)
            {
                return HttpNotFound();
            }
            return View(cardTemplate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content")] CardTemplate cardTemplate)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cardTemplate).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cardTemplate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var result = new ResultViewModel();
            try
            {
                db.CardTemplates.Remove(db.CardTemplates.Find(id));
                db.SaveChanges();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
