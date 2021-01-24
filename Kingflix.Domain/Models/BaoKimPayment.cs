using System;
using System.Collections.Generic;

namespace Kingflix.Domain.Models
{
    public class BankCallBackInformation
    {
        public int code { get; set; }
        public string[] message { get; set; }
        public int count { get; set; }
        public List<BankInformation> data { get; set; }
    }
    public class BankInformation
    {
        public int id { get; set; }
        public int bank_id { get; set; }
        public string bank_logo { get; set; }
        public string bank_name { get; set; }
        public string bank_short_name { get; set; }
        public string complete_time { get; set; }
        public string name { get; set; }
        public int type { get; set; }
    }

    public class PaymentAPIResult
    {
        public int code { get; set; }
        public string[] message { get; set; }
        public PaymentLink data = new PaymentLink();
    }
    public class PaymentLink
    {
        public string order_id { get; set; }
        public string redirect_url { get; set; }
        public string payment_url { get; set; }
        public string bank_account { get; set; }
        public string data_qr { get; set; }
    }

    public class PaymentCardResult
    {
        public int code { get; set; }
        public int count { get; set; }
        public string message { get; set; }
        public PaymentCardDetails data = new PaymentCardDetails();
    }
    public class PaymentCardDetails
    {
        public int user_id { get; set; }
        public int account_id { get; set; }
        public int amount { get; set; }
        public int freeze_amount { get; set; }
        public string pin { get; set; }
        public string serial { get; set; }
        public int type { get; set; }
        public int stat { get; set; }
        public string description { get; set; }
        public int fee_amount { get; set; }
        public int fee_display { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public string id { get; set; }
    }

    public class NotificationAPI
    {
        public OrderAPI order = new OrderAPI();
        public TxnAPI txn = new TxnAPI();
        public string sign { get; set; }
    }
    public class OrderAPI
    {
        public int id { get; set; }
        public int user_id {get;set;}
        public string mrc_order_id { get; set; }
        public int txn_id { get; set; }
        public int? ref_no { get; set; }
        public int? deposit_id { get; set; }
        public int merchant_id { get; set; }
        public double total_amount { get; set; }
        public double shipping_fee { get; set; }
        public double tax_fee { get; set; }
        public double? mrc_fee { get; set; }
        public string description { get; set; }
        public string url_success { get; set; }
        public string url_cancel { get; set; }
        public string url_detail { get; set; }
        public string stat { get; set; } //Trạng thái đơn hàng: "c"- Hoàn thành  |  "p" - Đang xử lý
        public string payment_version { get; set; }
        public string lang { get; set; }
        public int? bpm_id { get; set; }
        public int? accept_qrpay { get; set; }
        public int? accept_bank { get; set; }
        public int? accept_cc { get; set; }
        public int? accept_e_wallet { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string webhooks { get; set; }
        public string customer_name { get; set; }
        public string customer_email { get; set; }
        public string customer_phone { get; set; }
        public string customer_address { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class TxnAPI
    {
        public int user_id { get; set; }
        public int account_id { get; set; }
        public int deposit_id { get; set; }
        public double amount { get; set; }
        public double freeze_amount { get; set; }
        public double fee_display { get; set; }
        public string description { get; set; }
        public int? bank_ref_no { get; set; }
        public string ref_no { get; set; }
        public int stat { get; set; }
        public int type { get; set; }
        public string src_des { get; set; }
        public DateTime updated_at { get; set; }
        public DateTime created_at { get; set; }
        public int id { get; set; }
        public string mrc_order_id { get; set; }
        public double total_amount { get; set; }
    }

    public class OrderDetailsAPI
    {
        public int code { get; set; }
        public string[] message { get; set; }
        public int count { get; set; }
        public OrderDetailsDataAPI data = new OrderDetailsDataAPI();
    }

    public class OrderDetailsDataAPI
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string mrc_order_id { get; set; }
        public int? txn_id { get; set; }
        public int? ref_no { get; set; }
        public int? deposit_id { get; set; }
        public int merchant_id { get; set; }
        public double total_amount { get; set; }
        public double shipping_fee { get; set; }
        public double tax_fee { get; set; }
        public double? mrc_fee { get; set; }
        public string description { get; set; }
        public int? items { get; set; }
        public int? auth_code { get; set; }
        public string url_success { get; set; }
        public string url_cancel { get; set; }
        public string url_detail { get; set; }
        public string stat { get; set; } //Trạng thái đơn hàng: "c"- Hoàn thành  |  "p" - Đang xử lý
        public string payment_version { get; set; }
        public string lang { get; set; }
        public int? bpm_id { get; set; }
        public int? accept_qrpay { get; set; }
        public int? accept_bank { get; set; }
        public int? accept_cc { get; set; }
        public int? accept_e_wallet { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string webhooks { get; set; }
        public string customer_name { get; set; }
        public string customer_email { get; set; }
        public string customer_phone { get; set; }
        public string customer_address { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string redirect_url { get; set; }
        public string payment_url { get; set; }
    }

}