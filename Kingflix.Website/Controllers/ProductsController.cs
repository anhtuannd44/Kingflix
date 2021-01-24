using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Kingflix.Models;
using Kingflix.Domain.DomainModel;
using Kingflix.Models.ViewModel;
using Kingflix.Services.Data;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;

namespace Kingflix.Controllers
{
    public class ProductsController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Shop
        public ActionResult Index()
        {
            return View(db.Categories.Where(a => a.Type != TypeOfCategory.Netflix && a.Prices.Count != 0).ToList());
        }

        // GET: Shop/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }
        [HttpPost]
        public ActionResult GetPriceAndTotal(string categoryId, double Month, int qty)
        {
            var model = new PriceDetailsViewModel();
            var price = db.Price.Find(categoryId, Month);

            var flashSale = db.FlashSaleCategories.AsQueryable().Where(a => a.CategoryId == categoryId && a.Month == Month && a.FlashSales.TimeEnd >= DateTime.Now && a.FlashSales.TimeStart <= DateTime.Now).ToList();

            if (flashSale.Count > 0)
            {
                var pricesale = flashSale.OrderBy(a => a.PriceSale).FirstOrDefault().PriceSale;
                model.PriceShow = string.Format("{0:C0}", pricesale) + " <del class='small text-dark'>" + string.Format("{0:C0}", price.SetPrice) + "</del>";
                model.TotalShow = string.Format("{0:C0}", pricesale * qty) + " <del class='small text-dark'>" + string.Format("{0:C0}", price.SetPrice * qty) + "</del>";
                return Json(model, JsonRequestBehavior.DenyGet);
            }
            model.PriceShow = string.Format("{0:C0}", price.SetPrice);
            model.TotalShow = string.Format("{0:C0}", price.SetPrice * qty);

            return Json(model, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public ActionResult LoadMiniCart()
        {
            var cart = ShoppingCart.Cart;
            return PartialView("_MiniCartPartial", cart);
        }

        [HttpPost]
        public ActionResult AddToCart(string categoryId, double Month, int count, TypeOfAccount typeOfAccount, string user, string pass)
        {
            var result = new ResultViewModel();
            try
            {
                var cart = ShoppingCart.Cart;
                if (typeOfAccount == TypeOfAccount.UserAccount)
                {
                    cart.Add(categoryId, Month, count, typeOfAccount, user, pass);
                    result.status = "success";
                    result.message = "Thành công! Sản phẩm đã được thêm vào giỏ hàng";
                }
                else
                {
                    cart.Add(categoryId, Month, count, typeOfAccount, null, null);
                    result.status = "success";
                    result.message = "Thành công! Sản phẩm đã được thêm vào giỏ hàng";
                }
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Sản phẩm chưa được thêm vào giỏ hàng";
            }
            return Json(result, JsonRequestBehavior.DenyGet);

        }

        [HttpPost]
        public ActionResult RemoveFromCart(string categoryId, double Month)
        {
            var result = new ResultViewModel();
            try
            {
                var cart = ShoppingCart.Cart;
                cart.Remove(categoryId, Month);
                result.status = "success";
                result.message = "Thành công! Sản phẩm đã được xóa khỏi giỏ hàng";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xảy ra, vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Cart()
        {
            var cart = ShoppingCart.Cart;
            return View(cart);
        }

        public ActionResult CheckOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                var cart = ShoppingCart.Cart;
                return View(cart);
            }
            else
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("CheckOut") });
        }

        public ActionResult GetPaymentMethod()
        {
            var model = db.Payment.ToList();
            return PartialView("_PaymentMethodPartial", model);
        }
        public ActionResult GetBannerShop()
        {
            var model = db.Homepages.Where(a => a.Type == PictureHomeType.BannerShop || a.Type == PictureHomeType.SliderCategory).ToList();
            return PartialView("_BannerShopPartial", model);
        }
        public ActionResult GetBannerFixed()
        {
            var model = new List<Homepage>();
            Homepage banner = db.Homepages.Where(a => a.Title == "BANNER5").FirstOrDefault();
            model.Add(banner);
            banner = db.Homepages.Where(a => a.Title == "BANNER6").FirstOrDefault();
            model.Add(banner);
            return PartialView("_BannerFixedPartial", model);
        }

        public string GetTopText()
        {
            return db.Setting.Find("TOPTEXT").Content;
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
