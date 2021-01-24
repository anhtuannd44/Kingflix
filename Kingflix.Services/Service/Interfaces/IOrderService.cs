using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace Kingflix.Services.Interfaces
{
    public interface IOrderService
    {
        List<Order> GetOrderWithUserNotNull(OrderStatus? status, bool isAcceptPayment);
        IEnumerable<Order> GetOrderList(Expression<Func<Order, bool>> predicate = null);
        Order GetOrderById(string orderId);
        ResultViewModel UpdateOrder(string orderId, OrderStatus status, string cancelNote, bool confirmUserAccount, string apiId = null);
        int GetOrderNotiCount();
        OrderInformationViewModel CheckPromotion(string code, string categoryId, double month, int profile, string combo, string userId, bool isAuthenticated);
        bool IsPromotionValid(string promoId, string userId);
        Task<ResultViewModel> ConfirmPayment(OrderViewModel order, int? paymentMethod, TypeOfCategory Type, PaymentType PaymentType, string code, string serial, string telco, int amount, string userId);
        void UploadImageOrder(HttpPostedFileBase PaymentImage, string orderId);
        IEnumerable<OrderDetails> GetOrderDetailsList(Expression<Func<OrderDetails, bool>> predicate = null);
    }
}