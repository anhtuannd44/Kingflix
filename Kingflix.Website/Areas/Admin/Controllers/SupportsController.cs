using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SupportsController : Controller
    {
        private readonly ISupportService _supportService;
        public SupportsController(ISupportService supportService)
        {
            _supportService = supportService;
        }
        public ActionResult Index()
        {
            var model = _supportService.GetSupportList();
            return View(model);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Support support = _supportService.GetSupportById(id);
            if (support == null)
                return HttpNotFound();
            return View(support);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? SupportId, SupportStatus Status, string Reply)
        {
            if (SupportId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var item = _supportService.GetSupportById(SupportId);
            if (item == null)
                return HttpNotFound();
            item.Status = Status;
            item.Reply = Reply;
            _supportService.UpdateSupport(item);
            return RedirectToAction("Index");
        }
    }
}