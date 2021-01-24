﻿using Kingflix.Domain.DomainModel;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Domain.DomainModel.IdentityModel;
using System.Web;
using Kingflix.Utilities;
using System.Linq.Expressions;
using Kingflix.Services.Interfaces;

namespace Kingflix.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAPIService _apiService;
        private readonly IEmailService _emailService;
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<Profile> _profileRepository;
        private readonly IRepository<EmailHistory> _emailHistoryRepository;
        private readonly IRepository<Price> _priceRepository;
        private readonly IRepository<AppUser> _userRepository;
        private readonly IRepository<Voucher> _voucherRepository;
        private readonly IRepository<Voucher_Category> _voucherCategoryRepository;
        private readonly IRepository<OrderDetails> _orderDetailsRepository;
        private readonly IRepository<Payment> _paymentRepository;
        public OrderService(
            IUnitOfWork unitOfWork,
            IAPIService apiService,
            IEmailService emailService,
            IRepository<Order> orderRepository,
            IRepository<Profile> profileRepository,
            IRepository<EmailHistory> emailHistoryRepository,
            IRepository<Price> priceRepository,
            IRepository<AppUser> userRepository,
            IRepository<Voucher_Category> voucherCategoryRepository,
            IRepository<Voucher> voucherRepository,
            IRepository<OrderDetails> orderDetailsRepository,
            IRepository<Payment> paymentRepository)
        {
            _unitOfWork = unitOfWork;
            _apiService = apiService;
            _emailService = emailService;
            _orderRepository = orderRepository;
            _profileRepository = profileRepository;
            _emailHistoryRepository = emailHistoryRepository;
            _priceRepository = priceRepository;
            _userRepository = userRepository;
            _voucherRepository = voucherRepository;
            _voucherCategoryRepository = voucherCategoryRepository;
            _orderDetailsRepository = orderDetailsRepository;
            _paymentRepository = paymentRepository;
        }

        public List<Order> GetOrderWithUserNotNull(OrderStatus? status, bool isAcceptPayment)
        {
            var list = _orderRepository.GetAll().Where(a => a.UserId != null);
            if (status.HasValue)
                list = list.Where(a => a.Status == status);

            if (!isAcceptPayment)
                list = list.Where(a => a.File != null && a.Status == OrderStatus.Pending);
            return list.OrderByDescending(a => a.DateCreated).ToList();
        }
        public IEnumerable<Order> GetOrderList(Expression<Func<Order, bool>> predicate = null)
        {
            return _orderRepository.Filter(predicate);
        }
        public Order GetOrderById(string orderId)
        {
            return _orderRepository.GetById(orderId);
        }

        public ResultViewModel UpdateOrder(string orderId, OrderStatus status, string cancelNote, bool confirmUserAccount, string apiId = null)
        {
            var result = new ResultViewModel();
            var order = _orderRepository.GetById(orderId);
            if (order.Type == OrderType.GiftCode)
            {
                result = UpdateGiftcodeOrder(order, status);
            }
            else if (order.Type == OrderType.Order) //Nếu là Order mới
            {
                if (status == OrderStatus.Done) //Nếu là hành động duyệt đơn
                {
                    List<Profile> profileAvailable = GetAndAddUserToProfile(order.OrderDetails.ToList(), order.UserId);
                    if (profileAvailable != null)
                    {
                        profileAvailable.ForEach(a => _profileRepository.Update(a));
                        order.Status = OrderStatus.Done;
                        order.stat = apiId;
                        _orderRepository.Update(order);
                        order.IsSendMail = false;
                        var sendMail = _emailService.SendOrderSuccess(order);
                        if (sendMail)
                        {
                            order.IsSendMail = true;
                            var emailHistory = new EmailHistory()
                            {
                                Email = order.UserInformation.Email,
                                DateSend = DateTime.Now,
                                Type = EmailTypeHistory.OrderAccept
                            };
                            _emailHistoryRepository.Create(emailHistory);
                        }
                        _unitOfWork.SaveChanges();
                        result.status = "success";
                        result.message = "Thành công! Đơn đã được cập nhật!";
                    }
                    else
                    {
                        result.status = "error";
                        result.message = "Thất bại! Số Profile còn lại ít hơn Order. Vui lòng kiểm tra lại";
                    }
                }
                else
                    result = EditOrderUpdate(order, status, cancelNote);
            }
            //else //Nếu là các đơn gia hạn
            //{

            //}

            return result;
        }

        public ResultViewModel UpdateGiftcodeOrder(Order order, OrderStatus status)
        {
            order.Status = status;
            _orderRepository.Update(order);
            return new ResultViewModel() { status = "success", message = "Thành công! Đã duyệt đơn hàng sử dụng Giftcode!" };
        }
        public List<Profile> GetProfileAvailable(string categoryId)
        {
            return _profileRepository.GetAll().Where(a => a.Products.CategoryId == categoryId
                                                                     && a.Products.DateEnd.AddDays(-a.Products.Categories.DateOrderAccept.GetValueOrDefault(0)) > DateTime.Today //Check ngày được phép duyệt
                                                                     && string.IsNullOrEmpty(a.UserId)                 //Profile không có người dùng là Profile trống
                                                                     && a.Products.Status == ProductStatus.Active)     //Trạng thái tài khoản đang hoạt động
                                                                         .ToList();
        }
        public List<Profile> GetAndAddUserToProfile(List<OrderDetails> detail, string userId)
        {
            var slots = new List<Profile>();
            foreach (var item in detail)
            {
                var profiles = GetProfileAvailable(item.CategoryId);
                if (profiles.Count >= item.Count)
                {
                    if (item.CategoryId == Const.NETFLIX2)
                    {
                        var group = Cycle.King;
                        if (item.Month == 2 || item.Month == 4)
                            group = Cycle.Avenger;
                        else if (item.Month == 3 || item.Month == 6 || item.Month == 12)
                            group = Cycle.Mouse;
                        slots.AddRange(profiles.Where(a => a.Products.Cycle == group).Take(item.Count).ToList());
                    }
                    else
                    {
                        slots.AddRange(profiles.Take(item.Count));
                    }
                    foreach (var profile in slots)
                    {
                        var month = Convert.ToInt32(Math.Truncate(item.Month));
                        var second = (item.Month - month) * 30 * 24 * 60 * 60;

                        profile.DateModified = DateTime.Now;
                        profile.UserId = userId;
                        profile.DateStart = DateTime.Now;
                        profile.DateEnd = DateTime.Now.AddMonths(month).AddSeconds(second);
                        profile.OrderId = item.OrderId;
                    }
                }
                else
                    return null;
            }
            return slots;
        }
        public ResultViewModel EditOrderUpdate(Order order, OrderStatus status, string cancelNote = null)
        {
            var result = new ResultViewModel();
            try
            {
                order.Status = status;
                order.CancelNote = status == OrderStatus.Cencelled ? cancelNote : string.Empty;
                order.DateConfirm = DateTime.Now;
                order.IsSendMail = false;
                _orderRepository.Update(order);
                _unitOfWork.SaveChanges();
                result.status = "success";
                result.message = "Thành công! Đơn đã được cập nhật!";
            }
            catch
            {
                result.status = "error";
                result.message = "Thất bại! Có lỗi trong quá trình xử lý, vui lòng thử lại!";
            }
            return result;
        }
        public int GetOrderNotiCount()
        {
            return _orderRepository.GetAll().Where(a => a.Status == OrderStatus.Pending).Count();
        }
        public OrderInformationViewModel CheckPromotion(string code, string categoryId, double month, int profile, string combo, string userId, bool isAuthenticated)
        {
            var result = new OrderInformationViewModel();
            var voucher = new VoucherResultViewModel();
            result.Price = _priceRepository.Find(categoryId, month).SetPrice;
            result.PriceSale = 0;
            //Check Referral
            var userReferral = _userRepository.GetAll().Where(a => a.ReferralCode == code).FirstOrDefault();
            if (userReferral != null)
            {
                if (userReferral.Id == userId)
                {
                    result.Status = "notAuth";
                    result.Message = "Bạn không thể dùng mã cho chính bạn!";
                }
                else
                {
                    var checkOrder = _orderRepository.Filter(a => a.VoucherId == code && (a.Status == OrderStatus.Done || a.Status == OrderStatus.Pending)).Count();
                    if (checkOrder > 0)
                    {
                        result.Status = "error";
                        result.Message = "Mã khuyến mãi này đã được sử dụng hoặc hết hạn!";
                    }
                    else if (!isAuthenticated)
                    {
                        result.Status = "error";
                        result.Message = "Bạn phải đăng nhập mới áp dụng được mã giới thiệu!";
                    }
                    else
                    {
                        result.Status = "Success";
                        result.VoucherInformation.VoucherId = code;
                        result.VoucherInformation.VoucherName = "Ưu đãi chương trình giới thiệu";
                        result.VoucherInformation.VoucherPolicy = "<p> - Mã giới thiệu áp dụng cho lần đầu tiên mua hàng</p>";
                        voucher.Value = 50 - userReferral.PercentForReferral;
                        voucher.TypePromotion = VoucherType.Both;
                        voucher.TypeResult = VoucherResult.Referral;
                        voucher.MaxMinusValue = 30000;
                    }
                }

            }
            if (result.Status != "Success")
            {
                result.Status = "NotFound";
                var voucherItem = _voucherRepository.Filter(a => a.VoucherId == code || a.VoucherChilds.Where(b => b.VoucherId == code).Count() > 0).FirstOrDefault();
                if (voucherItem != null)
                    if (voucherItem.DateEnd.Date >= DateTime.Today && voucherItem.Status != VoucherStatus.Private)
                    {
                        result.VoucherInformation.VoucherId = voucherItem.VoucherId;
                        result.VoucherInformation.VoucherName = voucherItem.Name;
                        result.VoucherInformation.VoucherPolicy = voucherItem.PolicyContent;
                        voucher.Value = voucherItem.Value;
                        voucher.TypePromotion = voucherItem.Type;
                        voucher.VoucherFor = voucherItem.VoucherFor;
                        voucher.MaxMinusValue = voucherItem.MaxMoney;
                        voucher.TypeResult = VoucherResult.Voucher;

                        result.Status = "Success";
                    }
            }
            if (result.Status == "Success")
            {
                if (voucher.TypeResult == VoucherResult.Referral)
                {
                    var salePercent = result.Price * (voucher.Value / 100);
                    var salePrice = voucher.MaxMinusValue ?? salePercent;
                    result.PriceSale = salePercent > salePrice ? salePrice : salePercent;
                }
                else if (voucher.VoucherFor == VoucherFor.System && month == 1)
                {
                    if (categoryId == Const.NETFLIX0)
                        result.PriceSale = 19000;
                    else if (categoryId == Const.NETFLIX1)
                        result.PriceSale = 30000;
                    else if (categoryId == Const.NETFLIX2)
                        result.PriceSale = 40000;
                }
                else if (voucher.VoucherFor == VoucherFor.Normal)
                {
                    var voucherCategory = _voucherCategoryRepository.Find(code, categoryId);
                    if (voucherCategory != null)
                    {
                        if (voucher.TypePromotion == VoucherType.Money)
                        {
                            result.PriceSale = voucher.Value;
                        }
                        else if (voucher.TypePromotion == VoucherType.Percent)
                        {
                            result.PriceSale = result.Price * (voucher.Value / 100);
                        }
                        else
                        {
                            var salePercent = result.Price * (voucher.Value / 100);
                            var salePrice = voucher.MaxMinusValue ?? salePercent;
                            result.PriceSale = salePercent > salePrice ? salePrice : salePercent;
                        }
                    }
                    else
                    {
                        result.Status = "Error";
                        result.Message = "Mã khuyến mãi không áp dụng cho gói của bạn!";
                    }
                }
            }
            else if (!string.IsNullOrEmpty(code) && string.IsNullOrEmpty(result.Message))
            {
                result.Message = "Mã khuyến mãi hết hạn hoặc đã sử dụng!";
            }
            if (result.PriceSale > result.Price)
                result.PriceSale = result.Price;

            if (profile > 1)
                result.Price *= profile;

            result.Total = result.Price - result.PriceSale;

            if (!string.IsNullOrEmpty(combo))
            {
                var ComboArray = combo.Split(',');
                foreach (var item in ComboArray)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var comboDetailArray = item.Split('-');
                        var comboItem = _priceRepository.Find(comboDetailArray[0], Convert.ToDouble(comboDetailArray[1]));
                        if (comboItem != null)
                        {
                            result.Total += ((comboItem.Prices ?? comboItem.SetPrice) + (comboDetailArray[2] == "1" ? comboItem.Categories.GuaranteePrice : 0));
                            result.Price += ((comboItem.Prices ?? comboItem.SetPrice) + (comboDetailArray[2] == "1" ? comboItem.Categories.GuaranteePrice : 0));
                        }
                    }
                }
            }
            return result;
        }
        public bool IsPromotionValid(string promoId, string userId)
        {
            var order = _orderRepository.Filter(a => a.Status != OrderStatus.Cencelled && a.UserId == userId && a.VoucherId == promoId).ToList();
            if (order.Count != 0)
                return false;
            return true;
        }
        public async Task<ResultViewModel> ConfirmPayment(OrderViewModel order, int? paymentMethod, TypeOfCategory Type, PaymentType PaymentType, string code, string serial, string telco, int amount, string userId)
        {
            var result = new ResultViewModel();

            if ((string.IsNullOrEmpty(code) || string.IsNullOrEmpty(serial) || string.IsNullOrEmpty(telco)) && PaymentType == PaymentType.Card)
            {
                result.status = "error";
                result.message = "Bạn chưa nhập đủ dữ liệu để thanh toán. Vui lòng kiểm tra lại!";
            }
            else if (amount < order.Price && PaymentType == PaymentType.Card)
            {
                result.status = "error";
                result.message = "Thất bại! Mệnh giá thẻ phải lớn hơn hoặc bằng số tiền cần thanh toán!";
            }
            else
            {
                try
                {
                    var orderItem = new Order()
                    {
                        OrderId = "NETFLIX" + HelperFunction.RandomString(4),
                        DateCreated = DateTime.Now,
                        Status = OrderStatus.Pending,
                        Price = order.Price,
                        UserId = userId,
                        PaymentId = order.PaymentMethod,
                        VoucherId = order.VoucherId,
                        IsSendMail = false,
                        stat = "p",
                        AmountMoney = amount
                    };
                    result.OrderId = orderItem.OrderId;

                    var orderDetailsList = new List<OrderDetails>();
                    if (order.OrderDetails != null)
                    {
                        foreach (var item in order.OrderDetails)
                        {
                            var combo = _priceRepository.Find(item.CategoryId, item.Month);
                            if (combo != null)
                            {
                                orderDetailsList.Add(new OrderDetails()
                                {
                                    CategoryId = combo.CategoryId,
                                    CategoryName = combo.Categories.Name,
                                    OrderId = orderItem.OrderId,
                                    Month = combo.Month,
                                    Count = 1,
                                    ImageId = combo.Categories.ImageId,
                                    IsGuarantee = item.IsGuarantee
                                });
                            }
                        }
                    }
                    var user = _userRepository.GetById(userId);
                    var paymentId = _paymentRepository.Find(paymentMethod).Type;
                    if (PaymentType == PaymentType.Visa || PaymentType == PaymentType.QRCode || PaymentType == PaymentType.InternetBanking)
                    {
                        var createUrl = await _apiService.SendOrderToBaoKim(orderItem.OrderId, orderItem.Price, "Thanh toán đơn hàng " + orderItem.OrderId, "https://kingflix.vn/Order/Details?orderId=" + orderItem.OrderId, "https://kingflix.vn/Order/Details?orderId=" + orderItem.OrderId, "https://kingflix.vn/Order/Details?orderId=" + orderItem.OrderId, user.Email, user.PhoneNumber, string.IsNullOrEmpty(user.FullName) ? user.Email : user.FullName, user.Address, Convert.ToInt32(paymentId));
                        if (createUrl.status == "success")
                        {
                            result.status = "success";
                            result.message = "Thành công! Đang chuyển hướng đến cổng thanh toán";
                            result.redirect_url = createUrl.redirect_url;
                        }
                        else
                        {
                            result.status = "error";
                            result.message = "Thất bại. Vui lòng kiểm tra lại hoặc tiến hành đặt hàng lại";
                        }
                    }
                    else if (PaymentType == PaymentType.Card)
                    {
                        var paymentCard = await _apiService.PaymentCard(orderItem.OrderId, telco, amount, code, serial);
                        if (paymentCard.status == "success")
                        {
                            result.status = "success";
                            result.message = "Thành công! Đang chuyển hướng đến cổng thanh toán";
                            result.redirect_url = paymentCard.redirect_url;
                            orderItem.Status = OrderStatus.Pending;
                            orderItem.stat = "c";
                            orderItem.ApiOrderId = result.APIOrderId;
                            _orderRepository.Create(orderItem);
                            if (orderDetailsList.Count > 0)
                                _orderDetailsRepository.CreateRange(orderDetailsList);
                            _unitOfWork.SaveChanges();
                        }
                        else
                        {
                            result.status = "error";
                            result.message = paymentCard.message;
                        }
                    }
                    else if (PaymentType == PaymentType.EWallet || PaymentType == PaymentType.Bank)
                    {

                        result.status = "success";
                        result.message = "Thành công! Đơn hàng đã được xác nhận";
                        result.redirect_url = "/Order/PaymentOther?orderId=" + orderItem.OrderId;
                    }
                }
                catch (Exception ex)
                {
                    result.status = "error";
                    result.message = "Thất bại. Đơn hàng lỗi hoặc hết thời gian. Vui lòng đặt hàng lại!";
                }
            }
            return result;
        }
        public void UploadImageOrder(HttpPostedFileBase PaymentImage, string orderId)
        {
            try
            {
                int length = PaymentImage.ContentLength;
                byte[] image = new byte[length];
                PaymentImage.InputStream.Read(image, 0, length);
                var order = GetOrderById(orderId);
                order.File = image;
                _orderRepository.Update(order);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }
        public IEnumerable<OrderDetails> GetOrderDetailsList(Expression<Func<OrderDetails, bool>> predicate = null)
        {
            return _orderDetailsRepository.Filter(predicate);
        }
    }
}