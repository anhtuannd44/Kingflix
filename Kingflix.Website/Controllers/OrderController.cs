using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;
using Kingflix.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Kingflix.Controllers
{
    public class OrderController : Controller
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;
        public OrderController(IProductService productService, IOrderService orderService, IPaymentService paymentService, IUserService userService)
        {
            _productService = productService;
            _orderService = orderService;
            _paymentService = paymentService;
            _userService = userService;
        }
        public ActionResult Step1(string promoCode = null)
        {
            var userId = string.Empty;
            var isAuthenticated = User.Identity.IsAuthenticated;
            if (isAuthenticated)
                userId = User.Identity.GetUserId();

            if (Session["order"] == null) // Nếu giỏ hàng chưa được khởi tạo
                Session["order"] = new OrderViewModel();  // Khởi tạo Session["order"]

            Step1ViewModel model = new Step1ViewModel
            {
                ListCategoryDetails = _productService.GetCategoryNetflixDetails(),
                NetflixList = _productService.GetNetflixList(),
                OrderInformation = _orderService.CheckPromotion(promoCode, Const.NETFLIX2, 1, 1, "", userId, isAuthenticated)
            };
            return View(model);
        }
        public ActionResult GetCombo()
        {
            var model = _productService.GetUpsaleList();
            return PartialView("_ComboPartial", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckPromotion(string promoCode, string categoryId, double month, int profile, string combo)
        {
            var userId = string.Empty;
            var isAuthenticated = User.Identity.IsAuthenticated;
            if (isAuthenticated)
                userId = User.Identity.GetUserId();

            var model = _orderService.CheckPromotion(promoCode, categoryId, month, profile, combo, userId, isAuthenticated);
            return PartialView("_OrderInformationPartial", model);
        }

        [HttpPost]
        public ActionResult ConfirmStep1(string categoryId, double month, int profile, string promoCode, string combo)
        {
            if (Session["order"] == null)
                return Json(Url.Action("Step1"));

            var price = _productService.GetCategoryPrice(categoryId, month);
            if (price == null)
                return Json(Url.Action("Step1", new { promoCode }));

            OrderViewModel order = Session["order"] as OrderViewModel;

            order.VoucherId = promoCode;
            order.OrderType = OrderType.Order;
            order.OrderDetails.Add(new OrderDetails()
            {
                CategoryId = categoryId,
                Month = month,
                IsGuarantee = false,
                IsKingflixAccount = true,
                Count = profile
            });
            if (!string.IsNullOrEmpty(combo))
            {
                var ComboArray = combo.Split(',');
                foreach (var item in ComboArray)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var comboDetailArray = item.Split('-');
                        var comboItem = _productService.GetCategoryPrice(comboDetailArray[0], Convert.ToDouble(comboDetailArray[1]));
                        if (comboItem != null)
                            order.OrderDetails.Add(new OrderDetails()
                            {
                                CategoryId = comboDetailArray[0],
                                Month = Convert.ToDouble(comboDetailArray[1]),
                                IsGuarantee = comboDetailArray[2] == "1",
                                IsKingflixAccount = comboDetailArray[3] == "1",
                                UserAccount = comboDetailArray[3] == "1" ? comboDetailArray[4] : string.Empty,
                                UserPassword = comboDetailArray[3] == "1" ? comboDetailArray[5] : string.Empty,
                                Count = 1
                            });
                    }
                }
            }
            if (User.Identity.IsAuthenticated)
            {
                return Json(Url.Action("Payment", new { promoCode, categoryId, month, profile, combo }));
            }
            return Json(Url.Action("Register", new { returnUrl = Url.Action("Payment", new { promoCode }) }));
        }
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Step1");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        public ActionResult VoucherError(int? error)
        {
            if (error != 1)
                return HttpNotFound();
            else
                ViewBag.Error = "Ưu đãi đã được sử dụng hoặc đã hết hạn!";
            return View();
        }

        public ActionResult Payment(string promoCode, string categoryId, double month, int profile, string combo )
        {
            if (Session["order"] == null)
                return RedirectToAction("Step1");
            var userId = User.Identity.GetUserId();
            var userReferralCode = _userService.GetUserById(userId).ReferralCode;
            if (!string.IsNullOrEmpty(promoCode))
            {
                if (!_orderService.IsPromotionValid(promoCode, userId) || userReferralCode == promoCode)
                {
                    return RedirectToAction("VoucherError", new { error = 1 });
                }
            }
            var model = _paymentService.GetPaymentList();
            OrderViewModel orders = Session["order"] as OrderViewModel;
            orders.Price = _orderService.CheckPromotion(promoCode, categoryId, month, profile, combo, userId, true).Total;
            ViewBag.Total = orders.Price;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmPayment(int? paymentMethod, TypeOfCategory Type, PaymentType PaymentType, string code, string serial, string telco, int amount)
        {
            var result = new ResultViewModel();
            if ((PaymentType == PaymentType.EWallet || PaymentType == PaymentType.Bank) && !IsValidCaptcha(Request["g-recaptcha-response"]))
            {
                result.status = "error";
                result.message = "Vui lòng kiểm tra lại Recapcha";
                return Json(result);
            }

            if (Session["order"] == null || paymentMethod == null)
            {
                result.status = "success";
                result.redirect_url = Url.Action("Step1");
                return Json(result);
            }

            OrderViewModel order = Session["order"] as OrderViewModel;
            order.PaymentMethod = paymentMethod;

            if (!User.Identity.IsAuthenticated)
            {
                result.status = "success";
                result.message = "Thất bại. Bạn phải đăng nhập để tiến hành thanh toán";
                result.redirect_url = Url.Action("Step1");
                return Json(result);
            }
            else
            {
                var userId = User.Identity.GetUserId();
                result = await _orderService.ConfirmPayment(order, paymentMethod,Type, PaymentType, code, serial, telco, amount, userId);
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult PaymentOther(string orderId)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("PaymentOther", "Order", new { orderId }), IsRedirect = "True" });
            var userId = User.Identity.GetUserId();
            var model = _orderService.GetOrderById(orderId);
            if (model == null)
                return HttpNotFound();
            if (model.UserId != userId)
                return HttpNotFound();
            return View(model);
        }

        [HttpGet]
        public ActionResult Details(string orderId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                    return HttpNotFound();
                if (order.File != null)
                    ViewBag.IsImageUpload = "True";
                else
                    ViewBag.IsImageUpload = "False";
                if (order == null)
                    return RedirectToAction("Step1");
                return View(order);
            }
            return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", new { orderId }) });
        }

        //public async Task<ActionResult> CreateNewPaymentAPI(string orderId, PaymentType PaymentType)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        try
        //        {
        //            var userId = User.Identity.GetUserId();
        //            var userInformation = _appDb.Users.Find(userId);
        //            var order = _orderService.GetOrderById(orderId);
        //            if (order.UserId == userId)
        //            {
        //                if (order.DateCreated.AddMinutes(60) < DateTime.Now)
        //                {
        //                    var createUrl = await _apiService.SendOrderToBaoKim(order.OrderId, order.Price, "Thanh toán đơn hàng " + order.OrderId, "https://kingflix.vn", "https://kingflix.vn", "https://kingflix.vn", userInformation.Email, userInformation.PhoneNumber, string.IsNullOrEmpty(userInformation.FullName) ? userInformation.Email : userInformation.FullName, userInformation.Address, Convert.ToInt32(PaymentType));
        //                    if (createUrl.status == "success")
        //                    {
        //                        order.PaymentUrl = createUrl.redirect_url;
        //                        db.Set<Order>().AddOrUpdate(order);
        //                        await db.SaveChangesAsync();
        //                        return Redirect(createUrl.redirect_url);
        //                    }
        //                }
        //                else
        //                    return Redirect(order.PaymentUrl);
        //            }
        //            else
        //                return HttpNotFound();
        //        }
        //        catch
        //        {
        //            return RedirectToAction("Index", "Home");
        //        }

        //    }
        //    return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", new { orderId }) });
        //}

        //public ActionResult ChangePayment(string orderId)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var model = db.Payment.ToList();
        //        var order = db.Order.Find(orderId);
        //        if (order == null)
        //        {
        //            return HttpNotFound();
        //        }

        //        ViewBag.OrderId = orderId;
        //        ViewBag.PaymentId = order.PaymentId;
        //        ViewBag.Type = order.Payments.Type;
        //        return View(model);
        //    }
        //    return RedirectToAction("Register", "Account", new { returnUrl = Url.Action("ChangePayment", new { orderId }) });
        //}
        //public ActionResult UpdatePayment(int PaymentId, string orderId)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        var order = db.Order.Find(orderId);
        //        if (order == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        order.PaymentId = PaymentId;
        //        db.Entry(order).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Details", new { orderId = orderId, promoCode = order.VoucherId });
        //    }
        //    return RedirectToAction("Register", "Account", new { returnUrl = Url.Action("ChangePayment", new { orderId }) });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadImageOrder(HttpPostedFileBase PaymentImage, string orderId)
        {
            _orderService.UploadImageOrder(PaymentImage, orderId);
            return RedirectToAction("PaymentOther", new { orderId });
        }


        public bool IsValidCaptcha(string resp)
        {
            //string resp = Request["g-recaptcha-response"];
            var req = (HttpWebRequest)WebRequest.Create
                      ("https://www.google.com/recaptcha/api/siteverify?secret=6LcgztMZAAAAAOGEL11lf3YzfwE1zbRPTwIHFo7O&response=" + resp);
            using (WebResponse wResponse = req.GetResponse())
            {
                using (StreamReader readStream = new StreamReader(wResponse.GetResponseStream()))
                {
                    string jsonResponse = readStream.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    // Deserialize Json
                    CaptchaResult data = js.Deserialize<CaptchaResult>(jsonResponse);
                    if (Convert.ToBoolean(data.success))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
    public class CaptchaResult
    {
        public string success { get; set; }
    }
}