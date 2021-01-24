using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;

namespace Kingflix.Services
{

    public class BaoKimPayment
    {
        //URL checkout của BaoKim.vn
        public static string baokim_url = "https://api.baokim.vn/payment/api/v4/order/send";

        //Mã merchant site
        public static string merchant_id = "35289";    //Thay bằng mã merchant site bạn đã đăng ký trên BaoKim.vn

        //Mật khẩu bảo mật
        public static string secure_pass = "llnlc98lQWmCWu7rxtVdkTqVcqSh8IMv";    //Thay bằng mật khẩu giao tiếp giữa website của bạn với BaoKim.vn

        /**
         * Hàm thực hiện việc mã hóa, tạo khóa trên đường dẫn
         * @param messages xâu gốc
         * @return kết quả mã hóa
         * @throws Exception
         */
        public static string GetMD5Hash(string input)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(input);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();

            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }

            string md5string = s.ToString();
            return md5string;
        }

        /**
         * Hàm xây dựng url chuyển đến BaoKim.vn thực hiện thanh toán, trong đó có tham số mã hóa (còn gọi là public key)
         * @param $order_id                Mã đơn hàng
         * @param $business             Email tài khoản người bán
         * @param $total_amount            Giá trị đơn hàng
         * @param $shipping_fee            Phí vận chuyển
         * @param $tax_fee                Thuế
         * @param $order_description    Mô tả đơn hàng
         * @param $url_success            Url trả về khi thanh toán thành công
         * @param $url_cancel            Url trả về khi hủy thanh toán
         * @param $url_detail            Url chi tiết đơn hàng
         * @return url cần tạo
         */
        public static string createRequestUrl(string order_id, string business, string total_amount, string shipping_fee, string tax_fee, string order_description, string url_success, string url_cancel, string url_detail)
        {
            Hashtable order_params = new Hashtable();
            order_params.Add("merchant_id", merchant_id);
            order_params.Add("order_id", order_id);
            order_params.Add("business", business);
            order_params.Add("total_amount", total_amount);
            order_params.Add("shipping_fee", shipping_fee);
            order_params.Add("tax_fee", tax_fee);
            order_params.Add("order_description", order_description);
            order_params.Add("url_success", url_success);
            order_params.Add("url_cancel", url_cancel);
            order_params.Add("url_detail", url_detail);

            //Sắp xếp các tham số theo key để tiến hành mã hóa
            ICollection keyCollection = order_params.Keys;
            string[] keys = new string[keyCollection.Count];
            keyCollection.CopyTo(keys, 0);
            Array.Sort(keys);

            //Mã hóa tạo checksum
            string str_combined = secure_pass;
            foreach (string key in keys)
            {
                Object value = order_params[key];
                str_combined += value.ToString();
            }
            string checksum = GetMD5Hash(str_combined).ToUpper();


            //Tạo url redirect
            string redirect_url = baokim_url;

            if (redirect_url.IndexOf("?") == -1)
            {
                redirect_url += "?";
            }
            else if (redirect_url.Substring(redirect_url.Length - 1, 1) != "?" && redirect_url.IndexOf("&") == -1)
            {
                redirect_url += "&";
            }

            string url_params = "";
            foreach (string key in keys)
            {
                Object value = order_params[key];
                if (url_params == "")
                    url_params += key.ToString() + "=" + HttpContext.Current.Server.UrlEncode(value.ToString());
                else
                    url_params += "&" + key.ToString() + "=" + HttpContext.Current.Server.UrlEncode(value.ToString());
            }
            url_params += "&checksum=" + checksum;

            return redirect_url + url_params;
        }

        /**
         * Hàm thực hiện xác minh tính chính xác thông tin trả về từ BaoKim.vn
         * @param $_GET chứa tham số trả về trên url
         * @return true nếu thông tin là chính xác, false nếu thông tin không chính xác
         */
        public static bool verifyResponseUrl(NameValueCollection get_params)
        {
            //Sắp xếp các phần tử trong mảng tham số trả về theo key để mã hóa
            ICollection keyCollection = get_params.Keys;
            string[] keys = new string[keyCollection.Count];
            keyCollection.CopyTo(keys, 0);
            Array.Sort(keys);

            string str_combined = secure_pass;
            string checksum = "";
            foreach (string key in keys)
            {
                if (String.Compare(key, "checksum", true) != 0)
                {
                    Object value = get_params[key];
                    str_combined += value.ToString();
                }
                else
                {
                    checksum = get_params[key].ToString();
                }
            }

            //Mã hóa tạo check sum, so sánh với checksum gửi về từ BaoKim.vn
            string verify_checksum = GetMD5Hash(str_combined);

            if (String.Compare(verify_checksum, checksum, true) == 0)
                return true;

            return false;
        }
    }
}