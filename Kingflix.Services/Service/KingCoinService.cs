using Kingflix.Domain.DomainModel;
using Kingflix.Services.Interfaces;
using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel.IdentityModel;
using System.Collections.Generic;
using System.Linq;
using Kingflix.Domain.Enumerables;
using System;

namespace Kingflix.Services
{
    public class KingCoinService : IKingCoinService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<AppUser> _userRepository;
        private readonly IRepository<KingCoin> _kingCoinRepository;
        private readonly IRepository<Payment> _paymentRepository;
        public KingCoinService(
             IUnitOfWork unitOfWork,
             IRepository<AppUser> userRepository,
             IRepository<KingCoin> kingCoinRepository,
             IRepository<Payment> paymentRepository
             )
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _kingCoinRepository = kingCoinRepository;
            _paymentRepository = paymentRepository;
        }
        public List<KingCoin> GetKingCoinList()
        {
            return _kingCoinRepository.GetAll().ToList();
        }
        public void UpdateKingCoin(string id, CoinStatus status)
        {
            var kingcoin = _kingCoinRepository.Find(id);
            if (status == CoinStatus.Done)
            {
                var user = _userRepository.Find(kingcoin.UserId);
                user.KinCoin += kingcoin.Price;
                _userRepository.Update(user);
            }
            kingcoin.Status = status;
            kingcoin.DateConfirm = DateTime.Now;
            kingcoin.DateModified = DateTime.Now;
            _kingCoinRepository.Update(kingcoin);
            _unitOfWork.SaveChanges();
        }
        public KingCoin GetKingCoinItem(string id)
        {
            return _kingCoinRepository.GetById(id);
        }
        public List<Payment> GetListPayment()
        {
            return _paymentRepository.GetAll().ToList();
        }
        public void CreateKingCoin(KingCoin kingCoin)
        {
            _kingCoinRepository.Create(kingCoin);
            _unitOfWork.SaveChanges();
        }
    }
}