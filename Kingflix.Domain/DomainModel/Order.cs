using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("Order")]
    public partial class Order
    {
        public Order()
        {
            Profiles = new HashSet<Profile>();
            ExtendProfiles = new HashSet<ExtendProfile>();
            OrderDetails = new HashSet<OrderDetails>();
        }

        [Key]
        [Display(Name = "Mã đơn hàng")]
        public string OrderId { get; set; }
        public string ApiOrderId { get; set; }
        [Display(Name = "Tổng cộng")]
        public double Price { get; set; }

        public double AmountMoney { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Thời gian đặt hàng")]
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateConfirm { get; set; }
        public byte[] File { get; set; }
        public string UserId { get; set; }
        public bool IsSendMail {get;set;}
        public string PaymentUrl { get; set; }
        public OrderType Type { get; set; }
        [Display(Name = "Mã khuyến mãi")]
        public string VoucherId { get; set; }
        public int? PaymentId { get; set; }
        public string CancelNote { get; set; }
        public int? CardTemplate { get; set; }
        public bool IsDelivery { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryPhoneNumber { get; set; }
        public DateTime? DeliveryTime { get; set; }
        [AllowHtml]
        public string DeliveryMessage { get; set; }
        public bool IsUserGiftCode { get; set; }
        public string UserUseGiftCode { get; set; }
        public OrderStatus Status { get; set; }

        [ForeignKey("PaymentId")]
        public virtual Payment Payments { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser UserInformation { get; set; }

        public virtual ICollection<Profile> Profiles { get; set; }        
        public virtual ICollection<ExtendProfile> ExtendProfiles { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }


        [NotMapped]
        public DateTime DateConfirmShow
        {
            get
            {
                return DateConfirm ?? DateTime.MinValue;
            }
        }
    }
}