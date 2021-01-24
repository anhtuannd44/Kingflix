using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("Voucher_Category")]
    public partial class Voucher_Category
    {
        public Voucher_Category()
        {

        }

        [Key, Column(Order = 0)]
        public string VoucherId { get; set; }

        [Key, Column(Order = 1)]
        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Voucher Voucher { get; set; }
    }
}