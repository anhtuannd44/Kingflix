using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("Image")]
    public partial class Image
    {
        
        public Image()
        {
            Payments = new HashSet<Payment>();
            Homepages = new HashSet<Homepage>();
            Products = new HashSet<Product>();
            Categories = new HashSet<Category>();
            FlashSales = new HashSet<FlashSale>();
        }

        [StringLength(1000)]
        public string ImageId { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }

        
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<Homepage> Homepages { get; set; }
        public virtual ICollection<Product> Products{ get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<FlashSale> FlashSales { get; set; }

    }
}