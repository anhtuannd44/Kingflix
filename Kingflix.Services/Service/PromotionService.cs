using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using Kingflix.Services.Interfaces;
using Kingflix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Kingflix.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Voucher> _voucherRepository;
        private readonly IRepository<VoucherChild> _voucherChildRepository;
        private readonly IRepository<Voucher_Category> _voucherCategorydRepository;
        public PromotionService(
            IUnitOfWork unitOfWork,
            IRepository<Voucher> voucherRepository,
            IRepository<VoucherChild> voucherChildRepository,
            IRepository<Voucher_Category> voucherCateogoryRepository
            )
        {
            _unitOfWork = unitOfWork;
            _voucherRepository = voucherRepository;
            _voucherChildRepository = voucherChildRepository;
            _voucherCategorydRepository = voucherCateogoryRepository;
        }
        public IEnumerable<Voucher> GetVoucherList()
        {
            return _voucherRepository.Get();
        }
        public Voucher GetVoucherById(string voucherId)
        {
            return _voucherRepository.GetById(voucherId);
        }
        public void CreateVoucher(Voucher voucher, string[] VoucherCategory)
        {
            voucher.DateCreated = DateTime.Now;
            voucher.MaxMoney = voucher.Type != VoucherType.Both ? null : voucher.MaxMoney;
            _voucherRepository.Create(voucher);
            if (VoucherCategory != null)
            {
                foreach (var item in VoucherCategory)
                {
                    _voucherCategorydRepository.Create(new Voucher_Category() { VoucherId = voucher.VoucherId, CategoryId = item });
                }
            }
            _unitOfWork.SaveChanges();
        }
        public void UpdateVoucher(Voucher voucher, string[] VoucherCategory)
        {
            voucher.DateModified = DateTime.Now;
            voucher.MaxMoney = voucher.Type != VoucherType.Both ? null : voucher.MaxMoney;
            _voucherRepository.Update(voucher);
            if (VoucherCategory != null)
            {
                foreach (var item in _voucherCategorydRepository.Filter(a => a.VoucherId == voucher.VoucherId).ToList())
                {
                    _voucherCategorydRepository.Delete(item);
                }
                foreach (var item in VoucherCategory)
                {
                    _voucherCategorydRepository.Create(new Voucher_Category() { VoucherId = voucher.VoucherId, CategoryId = item });
                }
            }
            _unitOfWork.SaveChanges();
        }
        public void DeleteVoucher(string voucherId)
        {
            var item = _voucherRepository.GetById(voucherId);
            _voucherRepository.Delete(item);
            _unitOfWork.SaveChanges();
        }
        public VoucherChild GetVoucherChildById(string voucherId)
        {
            return _voucherChildRepository.GetById(voucherId);
        }
        public IEnumerable<VoucherChild> GetVoucherChildList(Expression<Func<VoucherChild, bool>> predicate = null)
        {
            return _voucherChildRepository.Filter(predicate);
        }
        public void CreateChildVoucher(int count, string voucherId)
        {
            for (int i = 1; i <= count; i++)
            {
                _voucherChildRepository.Create(new VoucherChild()
                {
                    VoucherChildId = voucherId + HelperFunction.RandomString(3) + i,
                    VoucherId = voucherId,
                    Status = VoucherStatus.Active
                });
            }
            _unitOfWork.SaveChanges();
        }
    }
}