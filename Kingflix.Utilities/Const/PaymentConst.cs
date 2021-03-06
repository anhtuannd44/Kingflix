namespace Kingflix.Utilities
{
    public partial class Const
    {
        //URL WEBSITE
        //public const string DOMAIN_URL = "https://20d7ab0e3a26.ngrok.io/";
        public const string DOMAIN_URL = "https://kingflix.vn/";
        //URL checkout của BaoKim.vn

        //Thanh toán online
        public const string BAOKIM_URL = "https://api.baokim.vn/payment/api/v4/order/send";
        public const string API_KEY = "qmGpEDTmDvJDzw65qWiciWkVkXyQhCtb";
        public const string API_SECRET = "KLQ0bT0Zbs4ePJy4lwhKlP09LiLoYNCR";
        public const string MERCHANT_ID = "35299";    //Thay bằng mã merchant site đăng ký trên BaoKim.vn
        public const string WEBHOOK_SUCCESS = DOMAIN_URL + "API/PaymentAPISuccess/";

        public const string URL_DETAILS = DOMAIN_URL + "Order/Details?orderid=";

        //Sandbox
        //public const string BAOKIM_URL = "https://sandbox-api.baokim.vn/payment/api/v4/order/send";
        //public const string API_KEY = "a18ff78e7a9e44f38de372e093d87ca1";
        //public const string API_SECRET = "9623ac03057e433f95d86cf4f3bef5cc";
        //public const string MERCHANT_ID = "8";    //Thay bằng mã merchant site đăng ký trên BaoKim.vn
        //public const string WEBHOOK_SUCCESS = DOMAIN_URL + "API/PaymentAPISuccess/";

        //Thanh toán bằng thẻ cào
        public const string CARD_PAYMENT_URL = "https://api.kingcard.online/kingcard/api/v1/strike-card";

        //ID Thẻ ATM Bao Kim
        public static int[] ATM_BAOKIM_ID_LIST => new int[] { 15, 293, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 60, 61, 62, 63, 64, 91, 94, 96, 97, 98, 101, 105, 112, 113, 115, 124, 129, 130, 131, 148, 150, 151, 152, 153, 154, 157, 158, 159 };
    }
}