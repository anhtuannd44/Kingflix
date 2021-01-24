using Kingflix.Domain.Enumerables;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("Page")]
    public partial class Page
    {
        [Key]
        public int PageId { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tiêu đề")]
        [StringLength(500)]
        public string Title { get; set; }

        public string Url { get; set; }

        public string Description { get; set; }

        public string MetaDescription { get; set; }

        public string MetaKeyword { get; set; }

        [Column(TypeName = "ntext")]
        [AllowHtml]
        public string PageContent { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }


        //status 1-Đã đăng 2-Chưa đăng 3-Đã xóa
        public PageStatus Status { get; set; }

    }
}