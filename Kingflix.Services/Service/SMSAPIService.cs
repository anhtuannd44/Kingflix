using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Kingflix.Utilities;
using System.Net;
using Kingflix.Services.Interfaces;

namespace Kingflix.Services
{
    public class SMSAPIService : ISMSAPIService
    {
        public JObject SendSMS(string phone, string message)
        {
            string URL = Const.SMS_API_URL + "SendMultipleMessage_V4_get?Phone=" + phone
                                           + "&Content=" + message
                                           + "&ApiKey=" + Const.SMS_API_KEY
                                           + "&SecretKey=" + Const.SMS_API_SECRET_KEY
                                           + "&IsUnicode=0"
                                           + "&Brandname=" + Const.SMS_API_BRAND_NAME
                                           + "&SmsType=2";
            string result = SendGetRequest(URL);
            if (result == null)
                return null;
            JObject ojb = JObject.Parse(result);
            return ojb;
        }
        //Lấy số dư tài khoản
        public double? GetBalance()
        {
            string URL = Const.SMS_API_URL + "GetBalance/" + Const.SMS_API_KEY + "/" + Const.SMS_API_SECRET_KEY;
            string result = SendGetRequest(URL);
            JObject ojb = JObject.Parse(result);
            int CodeResult = (int)ojb["CodeResponse"];
            if (CodeResult == 100) //trả về 100 là thành công
                return (long)ojb["Balance"];
            return null;
        }

        private string SendGetRequest(string RequestUrl)
        {
            Uri address = new Uri(RequestUrl);
            HttpWebRequest request;
            HttpWebResponse response = null;
            StreamReader reader;
            if (address == null) { throw new ArgumentNullException("address"); }
            try
            {
                request = WebRequest.Create(address) as HttpWebRequest;
                request.UserAgent = "Kingflix.vn";
                request.KeepAlive = false;
                request.Timeout = 15 * 1000;
                response = request.GetResponse() as HttpWebResponse;
                if (request.HaveResponse == true && response != null)
                {
                    reader = new StreamReader(response.GetResponseStream());
                    string result = reader.ReadToEnd();
                    result = result.Replace("</string>", "");
                    return result;
                }
            }
            catch (WebException wex)
            {

            }
            finally
            {
                if (response != null) { response.Close(); }
            }
            return null;
        }
    }
}