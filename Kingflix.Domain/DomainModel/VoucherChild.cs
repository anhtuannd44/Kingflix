using Kingflix.Domain.Enumerables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Kingflix.Domain.DomainModel
{
    [Table("VoucherChild")]
    public partial class VoucherChild
    {
        
        public VoucherChild()
        {
            
        }

        [Key]
        [Display(Name = "Mã Voucher")]
        public string VoucherChildId { get; set; }
        public VoucherStatus Status { get; set; }
        public string VoucherId { get; set; }
        public virtual Voucher Vouchers { get; set; }

        [NotMapped]
        public int UseCount { get; set; }
    }
}