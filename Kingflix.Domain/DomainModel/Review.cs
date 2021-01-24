using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Kingflix.Domain.DomainModel
{
    [Table("Review")]
    public partial class Review
    {
        
        public Review()
        {
        }

        [Key]
        public int ReviewId { get; set; }

        [Display(Name = "Nội dung")]
        [Required]
        public string Content { get; set; }

        [Required]
        public double Star { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Thời gian bình luận")]
        public DateTime DateCreated { get; set; }

        public DateTime? DateConfirm { get; set; }

        public int? ReplyFor { get; set; }

        public string UserId { get; set; }

        public int Like { get; set; }
        public byte[] Image { get; set; }

        public ReviewStatus Status { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser UserInformation { get; set; }

        [NotMapped]
        public List<Review> ReplyList { get; } = new List<Review>();
        //{
        //    get
        //    {
        //        using (var dbs = new AppDbContext())
        //        {
        //            return dbs.Review.Where(a => a.ReplyFor == ReviewId && a.Status == ReviewStatus.Accept).ToList();
        //        }
        //    }
        //}

        [NotMapped]
        public string ReplyForContent { get; set; }

        [NotMapped]
        public bool IsBought { get; set; }
        //{
        //    get
        //    {
        //        using (var dbs = new AppDbContext())
        //        {
        //            var order = dbs.Order.Where(a => a.Status == OrderStatus.Done && a.UserId == UserId);
        //            if (order == null)
        //                return false;
        //            return true;
        //        }

        //    }
        //}
    }
}