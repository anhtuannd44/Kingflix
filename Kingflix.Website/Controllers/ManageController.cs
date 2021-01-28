using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Data.Entity;
using Kingflix.Models.ViewModel;
using System.Collections.Generic;
using Kingflix.Domain.DomainModel;
using System.Net;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Utilities;
using Kingflix.Services.Interfaces;
using Kingflix.Services.Data;
using Kingflix.Website.Models;
using Kingflix.Services.Services;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Website.Controllers;
using AutoMapper;
using Kingflix.Domain.Abstract;
using Kingflix.Services.Data.Identity.Abstraction;
using NLog;

namespace Kingflix.Controllers
{
    [Authorize]
    public class ManageController : BaseExtendedController
    {
        private readonly IAuthenticationManager _authManager;
        private readonly AppDbContext db;
        private readonly IAPIService _apiService;

        public ManageController(
            IAppUserManager userManager,
             IAuthenticationManager authManager,
             IUnitOfWork unitOfWork,
             IMapper mapper,
            AppDbContext db,
            IAPIService apiService
            ) : base(unitOfWork, userManager, mapper)
        {
            this.db = db;
            _apiService = apiService;
            _authManager = authManager;
            _logger = LogManager.GetCurrentClassLogger();
        }

        [HttpPost]
        public string GetAvatar()
        {
            string base64 = "", imgSrc = "";
            var userId = User.Identity.GetUserId();
            var avatar = db.Users.Find(userId).Avatar;
            if (avatar != null)
            {
                base64 = Convert.ToBase64String(avatar);
                imgSrc = string.Format("data:image/gif;base64,{0}", base64);
            }
            else
            {
                imgSrc = "../../Content/Image/No_Picture.jpg";
            }
            return imgSrc;
        }

        public ActionResult UserInformation()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var model = new UserViewModel()
            {
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
                Gentle = user.Gentle,
                Address = user.Address,
                FeedbackStar = user.FeedbackStar,
                FeedbackContent = user.FeedbackContent,
                Referral = user.ReferralCode,
                Avatar = user.Avatar
            };
            return View(model);
        }

        public ActionResult UpdateUserInformation(UserViewModel user, HttpPostedFileBase avatars)
        {
            try
            {
                var userId = User.Identity.GetUserId();
                var item = db.Users.Find(userId);
                item.FullName = user.FullName;
                item.PhoneNumber = user.PhoneNumber;
                item.Birthday = user.Birthday;
                item.Gentle = user.Gentle;
                item.Address = user.Address;
                item.DateModified = DateTime.Now;

                if (avatars != null)
                {
                    int length = avatars.ContentLength;
                    byte[] image = new byte[length];
                    avatars.InputStream.Read(image, 0, length);
                    item.Avatar = image;
                }

                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch
            {
            }
            return RedirectToAction("UserInformation");
        }

        [HttpPost]
        public ActionResult SendFeedback(double star, string content)
        {
            var result = new ResultViewModel();
            try
            {
                var userId = User.Identity.GetUserId();
                var item = db.Users.Find(userId);
                item.FeedbackStar = star;
                item.FeedbackContent = content;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Nhận xét của bạn đã được ghi nhận";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng kiểm tra và thử lại.";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public PartialViewResult GetProfile(TypeOfCategory Type)
        {
            var userId = User.Identity.GetUserId();
            var profile = db.Profile.ToList().Where(a => a.UserId == userId && a.Products.Categories.Type == Type).ToList();
            ViewBag.IsPending = "False";
            var order = db.Order.Where(a => a.UserId == userId && a.Status == OrderStatus.WaitingForPay).Count();
            if (order > 0)
                ViewBag.IsPending = "True";
            if (Type == TypeOfCategory.Netflix)
                return PartialView("_ProfileNetflixPartial", profile);

            else
                return PartialView("_ProfileSpotifyPartial", profile);
        }


        public ActionResult OrderHistory()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Order.Where(a => a.UserId == userId && a.Type == OrderType.Order).ToList();
            return View(model);
        }
        public ActionResult ExtendHistory()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Order.Where(a => a.UserId == userId && a.Type == OrderType.Extend).ToList();
            return View(model);
        }
        public ActionResult GetExtendDetails(string orderId)
        {
            var model = db.ExtendProfiles.Where(a => a.OrderId == orderId).ToList();
            return PartialView("_ExtendDetailsPartial", model);
        }
        public ActionResult GetReferral()
        {
            var userId = User.Identity.GetUserId();
            var referralCode = db.Users.Find(userId).ReferralCode;
            var order = db.Order.Where(a => a.VoucherId == referralCode).ToList();
            var today = DateTime.Today;
            var thisMonth = today.Month;
            var thisYear = today.Year;
            var model = new ReferralReportViewModel()
            {
                ClickCountDay = order.Where(a => a.DateCreated.Date == today).Count(),
                ClickPendingDay = order.Where(a => a.Status == OrderStatus.WaitingForPay && a.DateCreated.Date == today).Count(),
                ClickSuccessDay = order.Where(a => a.Status == OrderStatus.Done && a.DateCreated.Date == today).Count(),
                ClickCountMonth = order.Where(a => a.DateCreated.Month == thisMonth && a.DateCreated.Year == thisYear).Count(),
                ClickPendingMonth = order.Where(a => a.Status == OrderStatus.WaitingForPay && a.DateCreated.Month == thisMonth && a.DateCreated.Year == thisYear).Count(),
                ClickSuccessMonth = order.Where(a => a.Status == OrderStatus.Done && a.DateCreated.Month == thisMonth && a.DateCreated.Year == thisYear).Count()
            };
            return PartialView("_ReferralPartial", model);
        }
        public ActionResult ReferralSetting()
        {
            var userId = User.Identity.GetUserId();
            var model = db.Users.Find(userId);
            return View(model);
        }

        [HttpPost]
        public JsonResult UpdateReferralPercent(int percent)
        {
            var result = new ResultViewModel();
            try
            {
                var userId = User.Identity.GetUserId();
                db.Users.Find(userId).PercentForReferral = percent;
                db.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Đã lưu cài đặt giới thiệu của bạn";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xảy ra khi lưu dữ liệu";
            }
            return Json(result, JsonRequestBehavior.DenyGet);

        }

        public ActionResult GetRevenue(int? timeView)
        {
            var userId = User.Identity.GetUserId();

            var result = new ChartViewModel()
            {
                datasets = new List<Datasets>()
                {
                    new Datasets()
                    {
                        label = "Click",
                    },
                    new Datasets()
                    {
                        label = "Thành công",
                    },
                }
            };

            var referralCode = db.Users.Find(userId).ReferralCode;
            var order = db.Order.Where(a => a.VoucherId == referralCode).ToList();

            var today = DateTime.Today;
            //Tất cả
            int record = 0;
            if (timeView == 0)
            {
                record = 7;
            }
            else if (timeView == 1)
            {
                //Tháng

                record = DateTime.DaysInMonth(today.Year, today.Month);
            }
            else
            {
                record = today.Month;
                order = order.Where(a => a.DateCreated.Year == today.Year).ToList();
            }

            result.labels = new string[record];
            result.datasets[0].data = new double[record];
            result.datasets[0].backgroundColor = new string[record];
            result.datasets[0].borderColor = new string[record];

            result.datasets[1].data = new double[record];
            result.datasets[1].backgroundColor = new string[record];
            result.datasets[1].borderColor = new string[record];

            for (int i = 0; i < record; i++)
            {
                if (timeView != null)
                {
                    var date = DateTime.MinValue;
                    if (timeView == 0)
                    {
                        date = today.AddDays(i - 6);
                    }
                    else
                    {
                        date = today.AddDays(i + 1 - record);
                    }
                    result.labels[i] = date.ToString("dd/MM");
                    result.datasets[0].data[i] = order.Where(a => a.DateCreated.Date == date.Date).Count();
                    result.datasets[1].data[i] = order.Where(a => a.DateCreated.Date == date.Date && a.Status == OrderStatus.Done).Count();
                }
                else
                {
                    result.labels[i] = (i + 1).ToString() + "/" + today.Year;
                    result.datasets[0].data[i] = order.Where(a => a.DateCreated.Month == (i + 1)).Count();
                    result.datasets[1].data[i] = order.Where(a => a.DateCreated.Month == (i + 1) && a.Status == OrderStatus.Done).Count();
                }
                result.datasets[0].backgroundColor[i] = "#fff";
                result.datasets[0].borderColor[i] = "rgba(247, 203, 45, 1)";
                result.datasets[1].backgroundColor[i] = "#fff";
                result.datasets[1].borderColor[i] = "rgba(140, 247, 77, 1)";
            }


            return Json(result, JsonRequestBehavior.DenyGet);
        }
        public ActionResult KingCoinHistory()
        {
            return View(db.KingCoin.ToList());
        }
        public ActionResult KingCoin()
        {
            ViewBag.PaymentId = new SelectList(db.Payment, "PaymentId", "Name");
            return View();
        }
        public ActionResult CreateKingCoin([Bind(Include = "Price,PaymentId")] KingCoin kingCoin, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();

                int length = Image.ContentLength;
                byte[] image = new byte[length];
                Image.InputStream.Read(image, 0, length);

                kingCoin.Image = image;
                kingCoin.Id = "BUYKINGCOIN" + HelperFunction.RandomString(4);
                kingCoin.DateCreated = DateTime.Now;
                kingCoin.Status = CoinStatus.Pending;
                kingCoin.UserId = userId;
                db.KingCoin.Add(kingCoin);
                db.SaveChanges();
            }
            ViewBag.PaymentId = new SelectList(db.Payment, "PaymentId", "Name");
            return RedirectToAction("KingCoin");
        }
        public ActionResult KingCoinDetails(string id)
        {
            var model = db.KingCoin.Find(id);
            return View(model);
        }
        [HttpPost]
        public JsonResult CancelKingCoin(string id)
        {
            var result = new ResultViewModel();
            try
            {
                var item = db.KingCoin.Find(id).Status = CoinStatus.Cencelled;
                db.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Đã hủy đơn nạp tiền của bạn";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng kiểm tra và thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult UploadImageKingCoin(string kingCoinId, HttpPostedFileBase file)
        {
            try
            {
                int length = file.ContentLength;
                byte[] image = new byte[length];
                file.InputStream.Read(image, 0, length);

                var order = db.KingCoin.Find(kingCoinId).Image = image;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("KingCoinDetails", new { id = kingCoinId });
        }
        public string GetKingCoin()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            return string.Format("{0:0,0 ₫}", user.KinCoin);
        }

        [HttpPost]
        public ActionResult GetPaymentDetails(int id, int?[] ProfileId, int Month)
        {
            var payment = db.Payment.Find(id);
            double total = 0;
            foreach (var item in ProfileId)
            {
                if (item != null)
                {
                    var profile = db.Profile.Find(item);
                    if (profile.Products.CategoryId == Const.NETFLIX2)
                    {
                        double price = 0;
                        var price1 = db.Price.Find(Const.NETFLIX2, 1).SetPrice;
                        var price3 = db.Price.Find(Const.NETFLIX2, 3).SetPrice;
                        var price6 = db.Price.Find(Const.NETFLIX2, 6).SetPrice;
                        var price12 = db.Price.Find(Const.NETFLIX2, 12).SetPrice;
                        if (Month < 3)
                            price += price1 * Month;
                        else if (Month < 6)
                            price = price3 + price1 * (Month - 3);
                        else if (Month < 12)
                        {
                            price = price6;
                            var checkMonth = Month - 6;
                            if (checkMonth >= 3)
                            {
                                price += price3;
                                checkMonth -= 3;
                            }
                            price += price1 * checkMonth;
                        }
                        else
                            price = db.Price.Find(Const.NETFLIX2, 12).SetPrice;
                        total += price;
                    }
                    else
                    {
                        double price = 0;
                        if (Month < 3)
                        {
                            price = db.Price.Find(profile.Products.CategoryId, 1).SetPrice;
                        }
                        else
                        {
                            price = db.Price.Find(profile.Products.CategoryId, 3).SetPrice;
                        }
                        total += price;
                    }
                }
            }
            ViewBag.Total = total;
            return PartialView("_PaymentDetailsPartial", payment);
        }

        public ActionResult Support()
        {
            var userId = User.Identity.GetUserId();
            return View(db.Supports.Where(a => a.UserId == userId).ToList());
        }

        public ActionResult CreateSupport()
        {
            var userId = User.Identity.GetUserId();
            var profileOfUser = db.Profile.ToList().Where(a => a.DateEnd.Date >= DateTime.Today && a.UserId == userId).ToList();
            var model = new Support()
            {
                UserId = userId,
                ProductList = profileOfUser.GroupBy(a => a.ProductId).Select(a => new Product() { ProductId = a.Key }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSupport([Bind(Include = "Title,Content,ProductId")] Support support, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                support.Status = SupportStatus.Pending;
                support.DateCreate = DateTime.Now;
                support.UserId = User.Identity.GetUserId();
                if (ImageUpload != null)
                {
                    int length = ImageUpload.ContentLength;
                    byte[] image = new byte[length];
                    ImageUpload.InputStream.Read(image, 0, length);
                    support.Image = image;
                }

                db.Supports.Add(support);
                db.SaveChanges();
                return RedirectToAction("Support");
            }

            return View(support);
        }

        // GET: Support/Edit/5
        public ActionResult EditSupport(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            Support support = db.Supports.Find(id);
            if (support == null)
            {
                return HttpNotFound();
            }
            if (support.UserId != userId)
                return HttpNotFound();
            return View(support);
        }

        [HttpPost]
        public ActionResult DeleteSupport(int id)
        {
            var result = new ResultViewModel();
            try
            {
                Support item = db.Supports.Find(id);
                item.Status = SupportStatus.Cancel;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
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

        public ActionResult Promotion()
        {
            var model = db.Voucher.ToList().Where(a => a.DateEnd.Date >= DateTime.Today && a.Status == VoucherStatus.Active && a.VoucherFor == VoucherFor.Normal).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Extend()
        {
            var userId = User.Identity.GetUserId();
            var ProductId = db.Profile.Where(a => a.UserId == userId && a.Products.Categories.Type == TypeOfCategory.Netflix && a.Products.CategoryId != Const.NETFLIX0).GroupBy(a => a.ProductId).Select(a => a.Key).ToArray();
            ViewBag.ProductId = ProductId;
            ViewBag.PaymentId = new SelectList(db.Payment, "PaymentId", "Name");
            return PartialView("_ExtendPartial");
        }

        [HttpPost]
        public ActionResult GetProfileExtend(string productId)
        {
            var userId = User.Identity.GetUserId();
            var profile = db.Profile.Where(a => a.ProductId == productId && a.UserId == userId).ToList();
            return PartialView("_ExtendProfilePartial", profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExtendConfirm(int?[] ProfileId, int Month, int PaymentId, double price)
        {
            var result = new ResultViewModel();
            var userId = User.Identity.GetUserId();

            if (ProfileId != null)
            {
                try
                {
                    var extend = new Order()
                    {
                        OrderId = "GIAHAN0" + HelperFunction.RandomString(5),
                        PaymentId = PaymentId,
                        Type = OrderType.Extend,
                        Price = price,
                        UserId = userId,
                        DateCreated = DateTime.Now,
                        Status = OrderStatus.WaitingForPay
                    };
                    db.Order.Add(extend);
                    db.SaveChanges();
                    foreach (var item in ProfileId)
                    {
                        if (item != null)
                        {
                            db.ExtendProfiles.Add(new ExtendProfile()
                            {
                                OrderId = extend.OrderId,
                                ProfileId = item.Value
                            });
                        }
                    }
                    db.SaveChanges();
                    result.status = "success";
                    result.message = "Thành công! Gia hạn của bạn xác nhận và đang xử lý";
                }
                catch (Exception ex)
                {
                    result.status = "error";
                    result.message = "Thất bại! Vui lòng kiểm tra và thử lại";
                }
            }
            else
            {
                result.status = "error";
                result.message = "Thất bại! Đã hết thời gian gia hạn";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult UseReferral()
        {
            var result = new ResultViewModel();
            try
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                if (user.ReferralCount >= 3 && !user.IsUseReferral)
                {
                    var spotifySlot = db.Profile.Where(a => a.Products.CategoryId == "SPOTIFY" && (string.IsNullOrEmpty(a.UserId) || a.DateEnd < DateTime.Now) && a.Products.Status != ProductStatus.Private).ToList();
                    if (spotifySlot.Count > 0)
                    {
                        var spotify = spotifySlot.FirstOrDefault();
                        spotify.DateModified = DateTime.Now;
                        spotify.UserId = userId;
                        spotify.DateStart = DateTime.Now;
                        spotify.DateEnd = DateTime.Now.AddMonths(3);
                        db.Entry(spotify).State = EntityState.Modified;
                        db.SaveChanges();

                        user.IsUseReferral = true;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                        result.status = "success";
                        result.message = "Thành công! Bạn đã nhận được 3 tháng Spotify miễn phí. Vui lòng kiểm tra ở Thông tin khách hàng";
                    }
                    else
                    {
                        result.status = "error";
                        result.message = "Xin lỗi, hiện tại hệ thống đang bận. Vui lòng thử lại sau ít phút";
                    }
                }
                else
                {
                    result.status = "error";
                    result.message = "Thất bại! Bạn chưa đủ điều kiện để nhận thưởng!";
                }
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi xảy ra, vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult GetSetting()
        {
            ViewBag.IsNull = "False";
            var model = db.Setting.Find("Policy");
            if (model == null)
            {
                ViewBag.IsNull = "True";
            }
            return PartialView("_PolicyPartial", model);
        }

        public ActionResult MemberShip()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            return View(user);
        }

        public ActionResult Giftcode()
        {
            var model = db.CardTemplates.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult GetCardTempate(int id)
        {
            var model = db.CardTemplates.Find(id);
            return PartialView("_CardTemplateDetailsPartial", model);
        }

        [HttpPost]
        public ActionResult CheckPriceGiftcode(string categoryId, string monthId)
        {
            var result = new ResultViewModel();
            result.price = "0 đ";
            try
            {
                double price = 0;
                var categoryArray = categoryId.Split('-');
                var monthArray = monthId.Split('-');
                for (int i = 0; i < categoryArray.Length; i++)
                {

                    if (!string.IsNullOrEmpty(monthArray[i]) && monthArray[i] != "0")
                    {
                        var category = db.Price.Find(categoryArray[i], Convert.ToDouble(monthArray[i]));
                        if (category != null)
                        {
                            price += category.SetPrice;
                        }
                    }
                }
                result.status = "success";
                result.price = string.Format("{0:0,0 đ}", price);
                result.message = "Thành công! Đã cập nhật đơn hàng!";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng kiểm tra và thử lại!";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SubmitOrderGiftcode(string categoryId, string monthId, bool isDelivery, string Name, string Address, string PhoneNumber, string Message, DateTime? TimeDelivery, int? CardTemplate)
        {
            var result = new ResultViewModel();
            result.price = "0 đ";
            try
            {
                double price = 0;
                var categoryArray = categoryId.Split('-');
                var monthArray = monthId.Split('-');
                for (int i = 0; i < categoryArray.Length; i++)
                {
                    if (!string.IsNullOrEmpty(monthArray[i]) && monthArray[i] != "0")
                    {
                        var category = db.Price.Find(categoryArray[i], Convert.ToDouble(monthArray[i]));
                        if (category != null)
                        {
                            price += category.SetPrice;
                        }
                    }
                }
                var userId = User.Identity.GetUserId();
                var order = new Order()
                {
                    OrderId = "GIFTCODE" + HelperFunction.RandomString(4),
                    Type = OrderType.GiftCode,
                    UserId = userId,
                    Price = price,
                    Status = OrderStatus.WaitingForPay,
                    DateCreated = DateTime.Now,
                    IsUserGiftCode = false
                };
                if (isDelivery)
                {
                    order.IsDelivery = true;
                    order.DeliveryName = Name;
                    order.DeliveryAddress = Address;
                    order.DeliveryPhoneNumber = PhoneNumber;
                    order.DeliveryTime = TimeDelivery;
                    if (CardTemplate.HasValue)
                    {
                        order.CardTemplate = CardTemplate;
                    }
                    else
                    {
                        order.DeliveryMessage = Message;
                    }
                }
                else
                    order.IsDelivery = false;
                db.Order.Add(order);
                db.SaveChanges();

                //Thêm Details 
                try
                {
                    List<OrderDetails> details = new List<OrderDetails>();
                    for (int i = 0; i < categoryArray.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(monthArray[i]) && monthArray[i] != "0")
                        {
                            var category = db.Price.Find(categoryArray[i], Convert.ToDouble(monthArray[i]));
                            if (category != null)
                            {
                                OrderDetails item = new OrderDetails()
                                {
                                    OrderId = order.OrderId,
                                    CategoryId = category.CategoryId,
                                    Month = category.Month,
                                    ImageId = category.Categories.ImageId,
                                    Count = 1,
                                    CategoryName = category.Categories.Name
                                };
                                details.Add(item);
                            }
                        }
                    }
                    using (var dbs = new AppDbContext())
                    {
                        dbs.OrderDetails.AddRange(details);
                        dbs.SaveChanges();
                    }

                    result.status = "success";
                    result.OrderId = order.OrderId;
                    result.message = "Thành công! Đơn hàng đã được lưu. Vui lòng thanh toán để sử dụng!";
                }
                catch
                {
                    using (var dbs = new AppDbContext())
                    {
                        dbs.Order.Remove(dbs.Order.Find(order.OrderId));
                        dbs.SaveChanges();
                    }
                    result.status = "error";
                    result.message = "Thất bại! Vui lòng kiểm tra và thử lại!";
                }
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Vui lòng kiểm tra và thử lại!";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public ActionResult LoadPaymentMethod(string orderId)
        {
            var model = db.Payment.ToList();
            ViewBag.OrderId = orderId;
            return PartialView("_PaymentDetailsPartial", model);
        }

        //[HttpPost]
        //public async Task<ActionResult> ConfirmPayment(int? paymentMethod, string orderId)
        //{
        //    var result = new ResultViewModel();
        //    try
        //    {
        //        var order = db.Order.Find(orderId);
        //        var userId = User.Identity.GetUserId();
        //        var user = db.Users.Find(userId);
        //        var createUrl = await _apiService.SendOrderToBaoKim(order.OrderId, order.Price, "Thanh toán đơn hàng " + order.OrderId, "https://kingflix.vn/Manage/giftcodelist", "https://kingflix.vn/Manage/giftcodelist", "https://kingflix.vn/Manage/giftcodelist", user.Email, user.PhoneNumber, string.IsNullOrEmpty(user.FullName) ? user.Email : user.FullName, user.Address, null);
        //        if (createUrl.status == "success")
        //        {
        //            result.status = "success";
        //            result.redirect_url = createUrl.redirect_url;
        //            result.message = "Thành công, trang sẽ được chuyển hướng để thanh toán";
        //        }
        //        else
        //        {
        //            result.status = "error";
        //            result.redirect_url = "#";
        //            result.message = "Thất bại! Vui lòng thử lại sau";
        //        }
        //    }
        //    catch
        //    {
        //        result.status = "error";
        //        result.redirect_url = "#";
        //        result.message = "Thất bại! Vui lòng thử lại sau";
        //    }
        //    return Json(result, JsonRequestBehavior.DenyGet);
        //}

        [HttpPost]
        public ActionResult OrderCancel(string orderId)
        {
            var result = new ResultViewModel();
            try
            {
                var order = db.Order.Find(orderId);
                order.Status = OrderStatus.Cancelled;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                result.status = "success";
                result.message = "Đã hủy đơn thành công!";
            }
            catch
            {
                result.status = "error";
                result.message = "Đơn hàng chưa được hủy. Vui lòng thử lại";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult GetCategoryList()
        {
            return PartialView("_CategoryListPartial", db.Categories.Where(a => a.Status == Status.Public && a.TypeOfAccount == TypeOfAccount.KingflixAccount).ToList());
        }

        public ActionResult GetMonthList(string categoryId)
        {
            return PartialView("_MonthListPartial", db.Price.Where(a => a.CategoryId == categoryId).ToList());
        }

        public ActionResult GiftcodeList()
        {
            var model = db.Order.Where(a => a.Type == OrderType.GiftCode);
            return View(model);
        }


        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(UserChangePasswordViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", GetErrorMessage.PasswordsDontMatch);
                return View(model);
            }

            AppUser user = GetCurrentUser();
            if (user != null)
            {
                bool correctPass = await _userManager.CheckPasswordAsync(user, model.OldPassword);
                if (!correctPass)
                {
                    ModelState.AddModelError("", GetErrorMessage.PasswordNotValid);
                    return View(model);
                }

                IdentityResult validPass = await _userManager.PasswordValidator.ValidateAsync(model.NewPassword);
                if (validPass.Succeeded)
                {
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(model.NewPassword);

                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        //_mailingRepository.PasswordChangedMail(user.Email);
                        return Json(result, JsonRequestBehavior.DenyGet);
                    }
                    else
                    {
                        AddErrorsFromResult(result);
                    }
                }
                else
                {
                    AddErrorsFromResult(validPass);
                }
            }

            ModelState.AddModelError("", GetErrorMessage.NullUser);
            return Json(model, JsonRequestBehavior.DenyGet);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        [NonAction]
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}