using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Kingflix.Domain.ViewModel
{
    public class Step1ViewModel
    {
        public List<CategoryNetflixDetails> ListCategoryDetails = new List<CategoryNetflixDetails>();
        public List<Category> NetflixList = new List<Category>();
        public OrderInformationViewModel OrderInformation = new OrderInformationViewModel();
    }
    public class OrderViewModel
    {
        public string VoucherId { get; set; }
        public int? PaymentMethod { get; set; }
        public OrderType OrderType { get; set; }
        public double Price { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    }
    public class OrderInformationViewModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public double Price { get; set; }
        public double PriceSale { get; set; }
        public double Total { get; set; }
        public VoucherViewModel VoucherInformation = new VoucherViewModel();
    }
    public class VoucherViewModel
    {
        public string VoucherId { get; set; }
        public string VoucherName { get; set; }
        [AllowHtml]
        public string VoucherPolicy { get; set; }
    }

    public class OrderList
    {
        public string CategoryId { get; set; }
        public int Count { get; set; }
        public double Month { get; set; }
    }
}