using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Domain.ViewModel;
using Kingflix.Services.Interfaces;
using System.Collections.Generic;
using System.Web;

namespace Kingflix.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Payment> _paymentRepository;
        public PaymentService(
            IUnitOfWork unitOfWork,
            IRepository<Payment> paymentRepository
            )
        {
            _unitOfWork = unitOfWork;
            _paymentRepository = paymentRepository;
        }
        public IEnumerable<Payment> GetPaymentList()
        {
            return _paymentRepository.GetAll();
        }
        public Payment GetPaymentById(int? id)
        {
            return _paymentRepository.GetById(id);
        }
        public void CreatePaymentMethod(Payment payment, HttpPostedFileBase file)
        {
            if (file != null)
            {
                int length = file.ContentLength;
                byte[] image = new byte[length];
                file.InputStream.Read(image, 0, length);
                payment.Logo = image;
            }
            _paymentRepository.Create(payment);
            _unitOfWork.SaveChanges();
        }
        public void UpdatePaymentMethod(Payment payment, HttpPostedFileBase logo)
        {
            var updateItem = _paymentRepository.GetById(payment.PaymentId);
            if (payment.Type == PaymentType.EWallet || payment.Type == PaymentType.Bank)
            {
                if (logo != null)
                {
                    int length = logo.ContentLength;
                    byte[] image = new byte[length];
                    logo.InputStream.Read(image, 0, length);
                    updateItem.Logo = image;
                }
                else
                {
                    updateItem.Name = payment.Name;
                    updateItem.AccountNumber = payment.AccountNumber;
                    updateItem.AccountName = payment.AccountName;
                    updateItem.Content = payment.Content;
                    updateItem.AccountAddress = payment.AccountAddress;
                    updateItem.Description = payment.Description;
                    updateItem.ImageId = payment.ImageId;
                    updateItem.Type = payment.Type;
                    updateItem.Status = payment.Status;
                }
            }
            else
            {
                updateItem.Status = payment.Status;
            }
            _paymentRepository.Update(updateItem);
            _unitOfWork.SaveChanges();
        }
        public ResultViewModel DeletePaymentMethod(int paymentId)
        {
            var result = new ResultViewModel();
            var item = _paymentRepository.Find(paymentId);
            if (item.Type == PaymentType.Bank || item.Type == PaymentType.EWallet)
            {
                _paymentRepository.Delete(item);
                _unitOfWork.SaveChanges();
                result.status = "success";
                result.message = "Thành công. Đã xóa dữ liệu";
            }
            else
            {
                result.status = "error";
                result.message = "Thất bại. Bạn không thể xóa phương thức API. Hãy tắt nếu không muốn sử dụng!";
            }
            return result;
        }
        public PaymentType GetPaymentType(int? paymentId)
        {
            return _paymentRepository.GetById(paymentId).Type;
        }
    }
}