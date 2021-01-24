using Kingflix.Domain.ViewModel;
using System.Threading.Tasks;

namespace Kingflix.Services.Interfaces
{
    public interface IAPIService
    {
        string GenerateUrlRedirect(string mrc_order_id, double total_amount, string description, string url_success, string url_cancel, string url_detail, string customer_email, string customer_phone, string customer_name, string customer_address, int? bpm_id);
        string GenerateUrlPaymentCard(string mrc_order_id, string telco, int amount, string code, string serial);
        string JwtEncode(string header, string payload, string secret);
        string CreateToken(string data, string secret);
        Task<ResultViewModel> SendOrderToBaoKim(string mrc_order_id, double total_amount, string description, string url_success, string url_cancel, string url_detail, string customer_email, string customer_phone, string customer_name, string customer_address, int? bpm_id);
        Task<ResultViewModel> PaymentCard(string mrc_order_id, string telco, int amount, string code, string serial);
    }
}