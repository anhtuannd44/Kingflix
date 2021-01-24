using Kingflix.Domain.DomainModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kingflix.Services.Data;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;

namespace Kingflix.Controllers
{
    public class RedeemController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();
        private readonly IEmailService _emailService;
        public RedeemController(
            IEmailService emailService
            )
        {
            _emailService = emailService;
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckGiftcode(string giftcode)
        {
            var result = new ResultViewModel();
            if (string.IsNullOrEmpty(giftcode))
            {
                result.status = "error";
                result.message = "Bạn chưa nhập Giftcode";
            }
            else if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var order = db.Order.Find(giftcode);
                    if (order == null)
                    {
                        result.status = "error";
                        result.message = "Giftcode không tồn tại. Vui lòng kiểm tra và thử lại";
                    }
                    else if (order.Type == OrderType.GiftCode && !order.IsUserGiftCode && order.Status == OrderStatus.Done)
                    {
                        var userId = User.Identity.GetUserId();
                        var user = db.Users.Find(userId);

                        var enoughSlot = true;
                        var isDone = true;
                        List<Profile> profileList = new List<Profile>();
                        foreach (var details in order.OrderDetails)
                        {
                            //Check Slot

                            var slot = db.Profile.Where(a => a.Products.CategoryId == details.CategoryId
                                                         && string.IsNullOrEmpty(a.UserId)
                                                         && a.Products.DateEnd >= DateTime.Now
                                                         && a.Products.Status == ProductStatus.Active
                                                    ).ToList();

                            if (slot.Count >= details.Count)
                            {
                                var i = 1;
                                var productId = "";
                                foreach (var item in slot)
                                {
                                    if (item.ProductId != productId && !item.SlotCheck)
                                    {
                                        var month = Convert.ToInt32(Math.Truncate(details.Month));
                                        var second = (details.Month - month) * 30 * 24 * 60 * 60;
                                        var currentProfile = db.Profile.Find(item.ProfileId);
                                        profileList.Add(new Profile()
                                        {
                                            ProfileId = item.ProfileId,
                                            DateCreated = item.DateCreated,
                                            DateModified = DateTime.Now,
                                            Name = item.Name,
                                            Pincode = item.Pincode,
                                            ProductId = item.ProductId,
                                            UserId = userId,
                                            DateStart = DateTime.Now,
                                            DateEnd = DateTime.Now.AddMonths(month).AddSeconds(second),
                                            OrderId = order.OrderId,
                                            Note = null
                                        });
                                        item.SlotCheck = true;
                                        i++;
                                    }
                                    if (i > details.Count)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                enoughSlot = false;
                                isDone = false;
                                result.status = "error";
                                result.message = "Thất bại! Số Profile còn lại ít hơn số Profile khách Order.";
                            }
                        }
                        if (enoughSlot)
                        {
                            try
                            {
                                using (var dbs = new AppDbContext())
                                {
                                    foreach (var item in profileList)
                                    {
                                        dbs.Entry(item).State = EntityState.Modified;
                                    }
                                    dbs.SaveChanges();
                                    isDone = true;
                                }

                            }
                            catch (Exception ex)
                            {
                                isDone = false;
                            }
                        }


                        if (isDone)
                        {
                            order.DateConfirm = DateTime.Now;
                            order.IsSendMail = false;
                            order.IsUserGiftCode = true;
                            order.UserUseGiftCode = userId;
                            db.Entry(order).State = EntityState.Modified;
                            db.SaveChanges();
                            var sendMail = _emailService.SendOrderSuccess(order);
                            if (sendMail)
                            {
                                order.IsSendMail = true;
                                db.Entry(order).State = EntityState.Modified;
                                db.SaveChanges();
                                db.EmailHistory.Add(new EmailHistory()
                                {
                                    Email = order.UserInformation.Email,
                                    DateSend = DateTime.Now,
                                    Type = EmailTypeHistory.OrderAccept
                                });
                                db.SaveChanges();
                            }
                            result.status = "success";
                            result.message = "Thành công! Bạn đã nhận được quà bằng Giftcode trị giá. Kiểm tra tại Trung tâm khách hàng";
                        }

                    }
                    else
                    {
                        result.status = "error";
                        result.message = "Giftcode không tồn tại hoặc đã được sử dụng. Vui lòng kiểm tra lại";
                    }
                }
                catch
                {
                    result.status = "error";
                    result.message = "Có lỗi xảy ra, vui lòng thử lại";
                }
            }
            else
            {
                result.status = "noauth";
                result.message = "Thất bại! Bạn phải đăng nhập để tiếp tục";
            }
            return Json(result, JsonRequestBehavior.DenyGet);
        }
    }
}