using Kingflix.Domain.Enumerables;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("Blog")]
    public partial class Blog
    {
        [Key]
        public int BlogId { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tiêu đề")]
        [StringLength(500)]
        public string Title { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeyword { get; set; }

        [Column(TypeName = "ntext")]
        [AllowHtml]
        public string BlogContent { get; set; }


        [StringLength(1000)]
        public string ImageId { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }


        [Display(Name = "Danh mục bài viết")]
        public int? BlogCategoryId { get; set; }

        //status 1-Đã đăng 2-Chưa đăng 3-Đã xóa
        public Status Status { get; set; }
        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }
        [ForeignKey("BlogCategoryId")]
        public virtual BlogCategory BlogCategory { get; set; }
        
    }
}