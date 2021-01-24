using Kingflix.Domain.Abstract;
using Kingflix.Domain.DomainModel;
using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Kingflix.Services.Interfaces
{
    public interface IPromotionService
    {
        IEnumerable<Voucher> GetVoucherList();
        Voucher GetVoucherById(string voucherId);
        void CreateVoucher(Voucher voucher, string[] VoucherCategory);
        void UpdateVoucher(Voucher voucher, string[] VoucherCategory);
        void DeleteVoucher(string voucherId);
        VoucherChild GetVoucherChildById(string voucherId);
        IEnumerable<VoucherChild> GetVoucherChildList(Expression<Func<VoucherChild, bool>> predicate = null);
        void CreateChildVoucher(int count, string voucherId);
    }
}