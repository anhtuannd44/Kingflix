using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Kingflix.Domain.DomainModel
{
    [Table("FlashSale")]
    public partial class FlashSale
    {
        public FlashSale()
        {
            FlashSaleCategories = new HashSet<FlashSaleCategory>();
        }

        [Key]
        public int FlashSaleId { get; set; }

        public string Title { get; set; }
        [StringLength(1000)]
        public string Cover { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }

        [ForeignKey("Cover")]
        public virtual Image Images { get; set; }
        public virtual ICollection<FlashSaleCategory> FlashSaleCategories { get; set; }
    }
}