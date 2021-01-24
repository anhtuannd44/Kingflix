using Kingflix.Domain.Enumerables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("OrderDetails")]
    public partial class OrderDetails
    {
        public OrderDetails()
        {

        }

        [Key]
        public int OrderDetailsId { get; set; }
        public string CategoryId { get; set; } //ForeignKey
        public string CategoryName { get; set; }
        public string ImageId { get; set; }
        public double Month { get; set; }
        public int Count { get; set; }
        public string UserAccount { get; set; }
        public string UserPassword { get; set; }
        public bool IsGuarantee { get; set; }
        public bool IsKingflixAccount { get; set; }
        public string OrderId { get; set; } //ForeignKey
        public OrderStatus Status { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Orders { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Categories { get; set; }

    }
}