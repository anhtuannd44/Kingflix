using Kingflix.Domain.Enumerables;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("Payment")]
    public partial class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
            KingCoins = new HashSet<KingCoin>();
        }

        [Key]
        public int PaymentId { get; set; }

        [Display(Name="Hình thức")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập số tài khoản hoặc số điện thoại")]
        [Display(Name = "Số tài khoản (Hoặc số điện thoại)")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tên chủ tài khoản")]
        [Display(Name = "Chủ tài khoản")]
        public string AccountName { get; set; }

        [Display(Name = "Chi nhánh đăng ký (bỏ qua nếu là Ví điện tử)")]
        public string AccountAddress { get; set; }
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [AllowHtml]
        public string Content { get; set; }

        public PaymentType Type { get; set; }

        public Status Status { get; set; }

        public byte[] Logo { get; set; } 
        public string ImageId { get; set; }
        public virtual Image Images { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<KingCoin> KingCoins { get; set; }
    }
}