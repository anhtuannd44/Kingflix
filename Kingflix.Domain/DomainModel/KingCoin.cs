using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("KingCoin")]
    public partial class KingCoin
    {
        public KingCoin()
        {
        }

        [Key]
        [Display(Name = "Mã giao dịch")]
        public string Id { get; set; }

        [Display(Name = "Số tiền nạp")]
        [Required(ErrorMessage = "Bạn chưa nhập số tiền cần nạp")]
        [Range(10000, Double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 10.000")]
        public double Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Thời gian đặt hàng")]
        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }
        public DateTime? DateConfirm { get; set; }

        public DateTime DateConfirmShow
        {
            get
            {
                return DateConfirm ?? DateTime.MinValue;
            }
        }

        public CoinStatus Status { get; set; }

        public byte[] File { get; set; }

        public string UserId { get; set; }

        public bool IsSendMail {get;set;}

        public byte[] Image { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser UserInformation { get; set; }
        public string VoucherId { get; set; }

        [Display(Name ="Chọn phương thức thanh toán của bạn")]
        public int? PaymentId { get; set; }

        public string CancelNote { get; set; }

        public virtual Payment Payments { get; set; }

    }
}