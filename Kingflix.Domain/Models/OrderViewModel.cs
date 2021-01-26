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
    public class OrderInformationViewModel
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public double Price { get; set; }
        public double PriceSale { get; set; }
        public double Total { get; set; }
        public VoucherViewModel VoucherInformation = new VoucherViewModel();
    }

    public class OrderViewModel
    {
        public string VoucherId { get; set; }
        public int? PaymentMethod { get; set; }
        public OrderType OrderType { get; set; }
        public double Price { get; set; }
        public List<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    }
   
    public class VoucherViewModel
    {
        public string VoucherId { get; set; }
        public string VoucherName { get; set; }
        [AllowHtml]
        public string VoucherPolicy { get; set; }
    }
    public class DataSendToBaoKim
    {
        public string mrc_order_id { get; set; }
        public double total_amount { get; set; }
        public string description { get; set; }
        public string url_success { get; set; }
        public string url_cancel { get; set; }
        public string url_detail { get; set; }
        public string customer_email { get; set; }
        public string customer_phone { get; set; }
        public string customer_name { get; set; }
        public string customer_address { get; set; }
        public int? bpm_id { get; set; }
    }
}