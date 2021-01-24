using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Data;

namespace Kingflix.Website.Areas.Admin.Controllers
{
    public class FlashSalesController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Admin/FlashSales
        public ActionResult Index()
        {
            var flashSale = db.FlashSale.Include(f => f.Images);
            return View(flashSale.ToList());
        }
        public ActionResult Create()
        {
            ViewBag.Cover = new SelectList(db.Image, "ImageId", "ImageId");
            return View();
        }

        // POST: Admin/FlashSales/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,Cover,TimeStart,TimeEnd")] FlashSale flashSale, string[] CategoryId, double?[] Month, double?[] PriceSale)
        {
            if (ModelState.IsValid)
            {
                db.FlashSale.Add(flashSale);
                db.SaveChanges();

                try
                {
                    for (int i = 0; i < CategoryId.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(CategoryId[i]) && Month[i] != null && PriceSale[i] != null)
                        {
                            //Check PriceSale
                            var price = db.Price.Find(CategoryId[i], Month[i]).SetPrice;
                            if (price > PriceSale[i])
                                db.FlashSaleCategories.Add(new FlashSaleCategory()
                                {
                                    FlashSaleId = flashSale.FlashSaleId,
                                    CategoryId = CategoryId[i],
                                    Month = Month[i].Value,
                                    PriceSale = PriceSale[i].Value
                                });
                        }
                    }
                    db.SaveChanges();
                }
                catch
                {
                    using (var dbs = new AppDbContext())
                    {
                        dbs.FlashSale.Remove(db.FlashSale.Find(flashSale.FlashSaleId));
                        dbs.SaveChanges();
                    }
                    return View(flashSale);
                }

                return RedirectToAction("Index");
            }
            return View(flashSale);
        }

        // GET: Admin/FlashSales/Edit/5
        public ActionResult Edit(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FlashSaleId,Title,Cover,TimeStart,TimeEnd")] FlashSale flashSale, string[] CategoryId, double?[] Month, double?[] PriceSale)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var item = db.FlashSale.Find(flashSale.FlashSaleId);
                    if (item == null)
                        return HttpNotFound();
                    item.Title = flashSale.Title;
                    item.Cover = flashSale.Cover;
                    item.TimeStart = flashSale.TimeStart;
                    item.TimeEnd = flashSale.TimeEnd;

                    //Các sản phẩm không có trong cũ
                    item.FlashSaleCategories.ToList().ForEach(a => item.FlashSaleCategories.Remove(a));

                    for (int i = 0; i < CategoryId.Length; i++)
                    {
                        if (item.FlashSaleCategories.Where(a=>a.CategoryId ==  CategoryId[i] && a.Month ==  Month[i]).Count() == 0)
                        {
                            if (!string.IsNullOrEmpty(CategoryId[i]) && Month[i] != null && PriceSale[i] != null)
                            {
                                //Check PriceSale
                                var price = db.Price.Find(CategoryId[i], Month[i]).SetPrice;
                                if (price > PriceSale[i])
                                    item.FlashSaleCategories.Add(new FlashSaleCategory()
                                    {
                                        FlashSaleId = flashSale.FlashSaleId,
                                        CategoryId = CategoryId[i],
                                        Month = Month[i].Value,
                                        PriceSale = PriceSale[i].Value
                                    });
                            }
                        }
                    }
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch
                {
                    return View(flashSale);
                }

                return RedirectToAction("Index");
            }
            return View(flashSale);
        }
        public ActionResult GetCategoryList()
        {
            return PartialView("_CategoryListPartial", db.Categories.Where(a => a.Status == Status.Public && a.Type != TypeOfCategory.Netflix).ToList());
        }

        public ActionResult GetMonthList(string categoryId)
        {
            return PartialView("_MonthListPartial", db.Price.Where(a => a.CategoryId == categoryId).ToList());
        }
        // GET: Admin/FlashSales/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Admin/FlashSales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FlashSale flashSale = db.FlashSale.Find(id);
            db.FlashSale.Remove(flashSale);
            db.SaveChanges();
            return RedirectToAction("Index");
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
