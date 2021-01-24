using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Kingflix.Domain.DomainModel
{
    [Table("ProductReplace")]
    public partial class ProductReplace
    {
        public ProductReplace()
        {

        }

        [Key]
        [Required(ErrorMessage = "Bạn chưa nhập Email")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng Email")]
        [Display(Name = "Email tài khoản")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Password")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập ngày hết hạn")]
        [Display(Name = "Ngày hết hạn")]
        public DateTime DateEnd { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        public ProductStatus Status { get; set; }

        [Display(Name = "Nhóm tài khoản")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }

        //[NotMapped]
        //public List<ProfileViewModel> ProfileList
        //{
        //    get
        //    {
        //        return Profiles.Where(a => a.ProductId == this.ProductId).Select(a => new ProfileViewModel() { ProfileId = a.ProfileId, Name = a.Name, Pincode = a.Pincode }).ToList();
        //    }
        //}

        [NotMapped]
        public int UserProfileCount
        {
            get
            {
                return Profiles.Where(a => a.ProductId == ProductId && !string.IsNullOrEmpty(a.UserId) && a.DateEnd > DateTime.Now).Count();
            }
        }

        [UIHint("Profile")]
        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual ICollection<Support> Supports { get; set; }
    }
}