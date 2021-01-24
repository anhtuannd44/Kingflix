using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Kingflix.Domain.DomainModel
{
    [Table("FlashSaleCategory")]
    public partial class FlashSaleCategory
    {
        public FlashSaleCategory()
        {

        }

        [Key]
        [Column(Order = 0)]
        public int FlashSaleId { get; set; }
        [Key]
        [Column(Order = 1)]
        public string CategoryId { get; set; }
        [Key]
        [Column(Order = 2)]
        public double Month { get; set; }
        public double PriceSale { get; set; }

        public virtual Price Prices { get; set; }
        public virtual FlashSale FlashSales { get; set; }

        [NotMapped]
        public List<Price> PriceList { get; set; } = new List<Price>();
    }
}