using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Services.Data;

namespace Kingflix.Controllers
{
    public class FlashSaleController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: FlashSale
        public ActionResult Index(int? flashSaleId)
        {
            FlashSale model = new FlashSale();
            if (flashSaleId == null)
                model = db.FlashSale.AsQueryable().Where(a => a.TimeStart <= DateTime.Now && a.TimeEnd >= DateTime.Now).FirstOrDefault();
            else
            {
                model = db.FlashSale.Find(flashSaleId);
            }    
            if (model == null)
                return HttpNotFound();
            List<FlashSale> listFlashSale = db.FlashSale.AsQueryable().Where(a => a.TimeEnd >= DateTime.Now && a.FlashSaleId != model.FlashSaleId).OrderBy(a => a.TimeStart).Take(3).ToList();
            listFlashSale.Add(model);
            ViewData["TopFlashSale"] = listFlashSale.OrderBy(a => a.TimeStart).ToList();
            return View(model);
        }

        // GET: FlashSale/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FlashSale flashSale = db.FlashSale.Find(id);
            if (flashSale == null)
            {
                return HttpNotFound();
            }
            return View(flashSale);
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
