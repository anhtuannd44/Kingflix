using Kingflix.Domain.ViewModel;
using System.Threading.Tasks;

namespace Kingflix.Services.Interfaces
{
    public interface IAPIService
    {
        string GenerateUrlPaymentCard(string mrc_order_id, string telco, int amount, string code, string serial);
        string JwtEncode(string header, string payload, string secret);
        string CreateToken(string data, string secret);
        Task<ResultViewModel> SendOrderToBaoKim(DataSendToBaoKim data);
        Task<ResultViewModel> PaymentCard(string mrc_order_id, string telco, int amount, string code, string serial);
    }
}