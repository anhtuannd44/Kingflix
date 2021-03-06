using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Utilities;
using Microsoft.AspNet.Identity;
using System.Web.Mvc;

namespace Kingflix.Website.Areas.Manage.Controllers
{
    [Authorize]
    public class RenewController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        public RenewController
            (
                IOrderService orderService,
                IProductService productService
            )
        {
            _orderService = orderService;
            _productService = productService;
        }
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var model = _productService.GetProfileList(a => a.UserId == userId &&
                                                            a.Products.Categories.TypeOfAccount == TypeOfAccount.KingflixAccount &&
                                                            a.Products.CategoryId != Const.NETFLIX0);
            return View(model);
        }
        public ActionResult GetPriceList(int profileId)
        {
            var profile = _productService.GetProfileById(profileId);
            var model = _productService.GetPriceListByCategoryId(profile.Products.CategoryId);
            return PartialView("_PriceListPartial", model);
        }
        [HttpPost]
        public ActionResult RenewConfirm(int ProfileId, double Month)
        {
            try
            {
                var profile = _productService.GetProfileById(ProfileId);
                var price = _productService.GetCategoryPrice(profile.Products.CategoryId, Month);
                ViewBag.Profile = profile.ProfileId;
                ViewBag.Total = price.Prices ?? price.SetPrice;
                return View();
            }
            catch
            {
                return View(nameof(Index));
            }
        }
    }
}