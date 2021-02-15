using Newtonsoft.Json.Linq;

namespace Kingflix.Services.Interfaces
{
    public interface ISMSAPIService
    {
        //Gửi tin nhắn
        JObject SendSMS(string phone, string message);
        //Lấy số dư tài khoản
        double? GetBalance();
    }
}