using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using System.Collections.Generic;

namespace Kingflix.Services.Interfaces
{
    public interface IKingCoinService
    {
        List<KingCoin> GetKingCoinList();
        void UpdateKingCoin(string id, CoinStatus status);
        KingCoin GetKingCoinItem(string id);
        List<Payment> GetListPayment();
        void CreateKingCoin(KingCoin kingCoin);
    }
}