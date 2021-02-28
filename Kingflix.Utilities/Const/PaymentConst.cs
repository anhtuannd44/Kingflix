namespace Kingflix.Utilities
{
    public partial class Const
    {
        //URL checkout của BaoKim.vn

        //Thanh toán online
        //public const string BAOKIM_URL = "https://api.baokim.vn/payment/api/v4/order/send";
        //public const string API_KEY = "qmGpEDTmDvJDzw65qWiciWkVkXyQhCtb";
        //public const string API_SECRET = "KLQ0bT0Zbs4ePJy4lwhKlP09LiLoYNCR";
        //public const string MERCHANT_ID = "35299";    //Thay bằng mã merchant site đăng ký trên BaoKim.vn
        //public const string WEBHOOK_SUCCESS = "https://kingflix.vn/API/PaymentAPISuccess/";

        //Sandbox
        public const string BAOKIM_URL = "https://sandbox-api.baokim.vn/payment/api/v4/order/send";
        public const string API_KEY = "a18ff78e7a9e44f38de372e093d87ca1";
        public const string API_SECRET = "9623ac03057e433f95d86cf4f3bef5cc";
        public const string MERCHANT_ID = "8";    //Thay bằng mã merchant site đăng ký trên BaoKim.vn
        public const string WEBHOOK_SUCCESS = "https://8f169fc9bc86.ngrok.io/API/PaymentAPISuccess/";

        //Thanh toán bằng thẻ cào
        public const string CARD_PAYMENT_URL = "https://api.kingcard.online/kingcard/api/v1/strike-card";

    }

}