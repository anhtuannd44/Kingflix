using Kingflix.Domain.Enumerables;
using System.Web.Mvc;

namespace Kingflix.Domain.ViewModel
{
    public class ResultViewModel
    {
        public string status { get; set; }
        public string message { get; set; }
        public string redirect_url { get; set; }
        public string price { get; set; }
        public string OrderId { get; set; }
        public string likecount { get; set; }
        public string APIOrderId { get; set; }
    }

    public class VoucherResultViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double? MaxMinusValue { get; set; }
        public double? MaxMinusValueForFirst { get; set; }
        [AllowHtml]
        public string VoucherPolicy { get; set; }
        public VoucherType TypePromotion { get; set; }
        public VoucherFor VoucherFor { get; set; }
        public VoucherResult TypeResult { get; set; }
        public string Status { get; set; }
    }



}