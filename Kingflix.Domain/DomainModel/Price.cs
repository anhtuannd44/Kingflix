using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Kingflix.Domain.DomainModel
{
    [Table("Price")]
    public partial class Price
    {
        
        public Price()
        {
            FlashSaleCategories = new HashSet<FlashSaleCategory>();
        }

        [Key]
        [Column(Order = 0)]
        public string CategoryId { get; set; }

        [Key]
        [Column(Order = 1)]
        public double Month { get; set; }

        //Giá gốc
        public double SetPrice { get; set; }

        //Giá khuyến mãi (bé hơn giá gốc)
        public double? Prices { get; set; }

        public string TextFixed { get; set; }

        public bool IsShow { get; set; }

        public virtual Category Categories { get; set; }

        [NotMapped]
        public double? PercentSale
        {
            get
            {
                if (Prices.HasValue)
                {
                    return Math.Round(100 - SetPrice / Prices.Value * 100);
                }
                return null;
            }
        }
        public virtual ICollection<FlashSaleCategory> FlashSaleCategories { get; set; }
    }
}