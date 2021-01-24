using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using System.Collections.Generic;
using System.Web;

namespace Kingflix.Services.Interfaces
{
    public interface IPaymentService
    {
        IEnumerable<Payment> GetPaymentList();
        PaymentType GetPaymentType(int? paymentId);
        Payment GetPaymentById(int? id);
        void CreatePaymentMethod(Payment payment, HttpPostedFileBase file);
        void UpdatePaymentMethod(Payment payment, HttpPostedFileBase file);
        ResultViewModel DeletePaymentMethod(int id);
    }
}