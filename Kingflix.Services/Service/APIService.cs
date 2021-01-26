using Kingflix.Domain.DomainModel;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kingflix.Domain.Abstract;
using Kingflix.Utilities;
using Kingflix.Domain.ViewModel;
using Microsoft.Ajax.Utilities;
using JsonWebToken;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.Models;

namespace Kingflix.Services
{
    public class APIService : IAPIService
    {
        public APIService()
        {
        }
        public string GenerateUrlRedirect(DataSendToBaoKim data)
        {
            JwtObject form_params = new JwtObject
                {
                    { "mrc_order_id", data.mrc_order_id },           // Mã đơn hàng
                    { "total_amount", data.total_amount },           // Tổng tiền
                    { "description", data.description },             // Tóm tắt đơn hàng
                    { "url_success", data.url_success },             // Link trả về khi hoàn thanh toán thành công
                    { "merchant_id", Const.merchant_id },       // Mã Merchant
                    { "url_detail", data.url_detail },               // Url chi tiết đơn hàng (redirect lại khi khách hủy đơn)
                    { "accept_bank", 1 },                       // Chấp nhận thanh toán bằng thẻ ATM ? (Chấp nhận: 1, Không chấp nhận: 0, default: 1)
                    { "accept_cc", 1 },                         // Chấp nhận thanh toán bằng thẻ Tín dụng ? (Chấp nhận: 1, Không chấp nhận: 0, default: 1)
                    { "accept_qrpay", 1 },                      // Chấp nhận thanh toán bằng QR code ? (Chấp nhận: 1, Không chấp nhận: 0, default: 0)
                    { "accept_e_wallet", 1 },                   // Chấp nhận thanh toán bằng Ví điện tử ? (Chấp nhận: 1, Không chấp nhận: 0, default: 1)
                    { "url_cancel", data.url_cancel },               //Url dùng để gửi thông báo cho website bán hàng, chat, ... khi đơn hàng thanh toán thành công, cho phép notify đến nhiều url, cách nhau bởi dấu ,
                    { "customer_email", data.customer_email },
                    { "webhooks", Const.webook_success },
                    { "customer_phone", data.customer_phone },
                    { "customer_name", data.customer_name },
                    { "customer_address", data.customer_address }
                };
            if (data.bpm_id != null)
                form_params.Add("bpm_id", data.bpm_id.Value);

            //Payload
            var payload = GetPayloadOfToken();

            //Ghép data vào payload
            form_params.ForEach(a => payload.Add(a));

            //Tạo Token
            var checksum = JwtEncode(GetHeaderOfToken().ToString(), payload.ToString(), Const.API_SECRET);

            //Thêm Token vào data send
            payload.Add("jwt", checksum);

            //Tạo url redirect
            string redirect_url = Const.baokim_url;

            if (redirect_url.IndexOf("?") == -1)
                redirect_url += "?";
            else if (redirect_url.Substring(redirect_url.Length - 1, 1) != "?" && redirect_url.IndexOf("&") == -1)
                redirect_url += "&";

            redirect_url += "mrc_order_id=" + form_params["mrc_order_id"].Value + "&";
            redirect_url += "total_amount=" + form_params["total_amount"].Value + "&";
            redirect_url += "description=" + form_params["description"].Value + "&";
            redirect_url += "url_success=" + form_params["url_success"].Value + "&";
            redirect_url += "merchant_id=" + form_params["merchant_id"].Value + "&";
            redirect_url += "url_detail=" + form_params["url_detail"].Value + "&";
            redirect_url += "accept_bank=" + form_params["accept_bank"].Value + "&";
            redirect_url += "accept_cc=" + form_params["accept_cc"].Value + "&";
            redirect_url += "accept_qrpay=" + form_params["accept_qrpay"].Value + "&";
            redirect_url += "accept_e_wallet=" + form_params["accept_e_wallet"].Value + "&";
            redirect_url += "url_cancel=" + form_params["url_cancel"].Value + "&";
            redirect_url += "webhooks=" + form_params["webhooks"].Value + "&";
            redirect_url += "customer_email=" + form_params["customer_email"].Value + "&";
            redirect_url += "customer_phone=" + form_params["customer_phone"].Value + "&";
            redirect_url += "customer_name=" + form_params["customer_name"].Value + "&";
            redirect_url += "customer_address=" + form_params["customer_address"].Value + "&";

            if (data.bpm_id != null)
                redirect_url += "bpm_id=" + form_params["bpm_id"].Value + "&";

            redirect_url += "jwt=" + payload["jwt"].Value;

            return redirect_url;
        }

        public string GenerateUrlPaymentCard(string mrc_order_id, string telco, int amount, string code, string serial)
        {
            JwtObject form_params = new JwtObject
                {
                    { "mrc_order_id", mrc_order_id },           // Mã đơn hàng
                    { "telco", telco },             // Link trả về khi hoàn thanh toán thành công
                    { "amount", amount },               // Url chi tiết đơn hàng (redirect lại khi khách hủy đơn)
                    { "code", code },                       // Chấp nhận thanh toán bằng thẻ ATM ? (Chấp nhận: 1, Không chấp nhận: 0, default: 1)
                    { "serial", serial },                         // Chấp nhận thanh toán bằng thẻ Tín dụng ? (Chấp nhận: 1, Không chấp nhận: 0, default: 1)
                    { "webhooks", "https://kingflix.vn/API/PaymentCardSuccess/" },                      // Chấp nhận thanh toán bằng QR code ? (Chấp nhận: 1, Không chấp nhận: 0, default: 0)
                };

            var payload = GetPayloadOfToken();
            //Ghép data vào payload
            form_params.ForEach(a => payload.Add(a));

            //Tạo Token
            var checksum = JwtEncode(GetHeaderOfToken().ToString(), payload.ToString(), Const.API_SECRET);

            //Thêm Token vào data send
            payload.Add("jwt", checksum);

            //Tạo url redirect
            string redirect_url = Const.card_payment_url;

            if (redirect_url.IndexOf("?") == -1)
                redirect_url += "?";
            else if (redirect_url.Substring(redirect_url.Length - 1, 1) != "?" && redirect_url.IndexOf("&") == -1)
                redirect_url += "&";

            redirect_url += "mrc_order_id=" + form_params["mrc_order_id"].Value + "&";
            redirect_url += "telco=" + form_params["telco"].Value + "&";
            redirect_url += "amount=" + form_params["amount"].Value + "&";
            redirect_url += "code=" + form_params["code"].Value + "&";
            redirect_url += "serial=" + form_params["serial"].Value + "&";
            redirect_url += "webhooks=" + form_params["webhooks"].Value + "&";
            redirect_url += "jwt=" + payload["jwt"].Value;
            return redirect_url;
        }
        public JwtObject GetHeaderOfToken()
        {
            //Header
            return new JwtObject { { "alg", "HS256" } };

        }
        public JwtObject GetPayloadOfToken()
        {
            //Payload
            return new JwtObject
            {
                { "iat", (double)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds },
                { "jti", HelperFunction.RandomString(7) },
                { "iss", Const.API_KEY },
                { "nbf", (double)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds },
                { "exp", (double)(DateTime.Now.AddHours(1) - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime()).TotalSeconds }
            };
        }
        public string JwtEncode(string header, string payload, string secret)
        {
            var headerEncode = HelperFunction.EncodeTo64(header);
            var payloadEncode = HelperFunction.EncodeTo64(payload);
            var data = headerEncode + "." + payloadEncode;
            return data + "." + CreateToken(data, secret);
        }
        public string CreateToken(string data, string secret)
        {
            secret = secret ?? "";
            byte[] keyByte = Encoding.UTF8.GetBytes(secret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(data);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage).Replace("=", "").Replace("/", "_").Replace("+", "-");
            }
        }
        public async Task<ResultViewModel> SendOrderToBaoKim(DataSendToBaoKim dataSend)
        {
            var result = new ResultViewModel();
            try
            {
                string redirect_url = GenerateUrlRedirect(dataSend);

                //Gửi Event đến Bảo Kim và nhận data trả về
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.PostAsync(redirect_url, null);
                HttpContent content = response.Content;
                var a = content.ReadAsStringAsync();
                var dataResponsed = await content.ReadAsAsync<PaymentAPIResult>();

                //Nếu thành công!
                if (dataResponsed.code == 0)
                {
                    result.status = "success";
                    result.message = "Thành công! Đơn hàng của bạn được giữ trong 60 phút. Hãy hoàn thành thanh toán của bạn!";
                    result.redirect_url = dataResponsed.data.payment_url;
                    result.APIOrderId = dataResponsed.data.order_id;
                }
                else
                {
                    result.status = "error";
                    result.message = "Thất bại! Vui lòng kiểm tra lại đơn hàng hoặc tiến hành đặt lại đơn hàng!";
                }
            }
            catch (Exception ex)
            {
                result.status = "error";
                result.message = "Có lỗi khi gửi dữ liệu. Vui lòng thử lại";
            }
            return result;
        }
        public async Task<ResultViewModel> PaymentCard(string mrc_order_id, string telco, int amount, string code, string serial)
        {
            var result = new ResultViewModel();
            try
            {
                string redirect_url = GenerateUrlPaymentCard(mrc_order_id, telco, amount, code, serial);

                //Gửi data đến BaoKim Card và nhận dữ liệu trả về
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.PostAsync(redirect_url, null);
                HttpContent content = response.Content;
                var data = await content.ReadAsAsync<PaymentCardResult>();

                if (data.code == 0)
                {
                    result.status = "success";
                    result.message = "Thành công! Đơn hàng của bạn đã được thanh toán!";
                    result.redirect_url = "/Order/Details?orderid=" + mrc_order_id;
                    result.APIOrderId = data.data.id;
                }
                else
                {
                    result.status = "error";
                    result.message = data.message;
                }
            }
            catch (Exception ex)
            {
                result.status = "error";
                result.message = "Có lỗi khi gửi dữ liệu. Vui lòng thử lại";
            }
            return result;
        }
    }
}