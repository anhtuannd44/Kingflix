namespace Kingflix.Utilities
{
    public partial class Const
    {
        //URL checkout của BaoKim.vn
        
        //Thanh toán online
        public const string baokim_url = "https://api.baokim.vn/payment/api/v4/order/send";
        public const string API_KEY = "qmGpEDTmDvJDzw65qWiciWkVkXyQhCtb";
        public const string API_SECRET = "KLQ0bT0Zbs4ePJy4lwhKlP09LiLoYNCR";
        public const string merchant_id = "35299";    //Thay bằng mã merchant site đăng ký trên BaoKim.vn
        public const string webook_success = "https://kingflix.vn/API/PaymentAPISuccess/";

        //Thanh toán bằng thẻ cào
        public const string card_payment_url = "https://api.kingcard.online/kingcard/api/v1/strike-card";

        //Sandbox
        public const string baokim_url_sandbox = "https://sandbox-api.baokim.vn/payment/api/v4/order/send";
        public const string API_KEY_SANDBOX = "a18ff78e7a9e44f38de372e093d87ca1";
        public const string API_SECRET_SANDBOX = "9623ac03057e433f95d86cf4f3bef5cc";
        public const string merchant_id_sandbox = "8";
        
    }

}