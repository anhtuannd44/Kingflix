using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("Support")]
    public partial class Support
    {
        public Support()
        {
           
        }

        [Key]
        public int SupportId { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Tiêu đề")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Nội dung")]
        [Display(Name = "Nội dung yêu cầu")]
        public string Content { get; set; }

        [Display(Name = "Trả lời")]
        public string Reply { get; set; }

        public byte[] Image { get; set; }

        [Display(Name="Người báo lỗi")]
        public string UserId { get; set; } 

        [Display(Name = "Thời gian yêu cầu")]
        public DateTime DateCreate { get; set; }
        public DateTime? DateModified { get; set; }

        [Required(ErrorMessage = "Phải chọn ít nhất 1 tài khoản báo lỗi")]
        [Display(Name = "Tài khoản báo lỗi")]
        public string ProductId { get; set; }

        [NotMapped]
        public List<Product> ProductList = new List<Product>();
        [ForeignKey("UserId")]
        public virtual AppUser UserInformation { get; set; }
        public virtual Product Products { get; set; }

        public SupportStatus Status { get; set; }
    }
}