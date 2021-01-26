using Kingflix.Domain.ViewModel;
using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Kingflix.Domain.DomainModel
{
    [Table("Product")]
    public partial class Product
    {
        public Product()
        {
            Profiles = new HashSet<Profile>();
            Supports = new HashSet<Support>();
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
        public int? ReplaceProduct { get; set; }

        [Display(Name = "Chu kỳ")]
        public Cycle Cycle { get; set; }

        [Display(Name = "Loại tài khoản")]
        public string CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Categories { get; set; }

        [StringLength(1000)]
        public string ImageId { get; set; }
        [ForeignKey("ImageId")]
        public virtual Image Image { get; set; }

        [Display(Name = "Thứ tự nhóm tài khoản")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int GroupId { get; set; }
        public string ParentId { get; set; }

        [NotMapped]
        public Product Parent { get; set; }

        [NotMapped]
        public bool IsSelected { get; set; }

        [NotMapped]
        public bool IsProfileError
        {
            get
            {
                var support = Supports.AsQueryable().Where(a => a.ProductId == ProductId && a.Status == SupportStatus.Pending);
                if (support.Count() == 0)
                    return false;
                return true;
            }
        }

        [NotMapped]
        public List<ProfileViewModel> ProfileList
        {
            get
            {
                return Profiles.Where(a => a.ProductId == this.ProductId).Select(a => new ProfileViewModel() { ProfileId = a.ProfileId, Name = a.Name, Pincode = a.Pincode }).ToList();
            }
        }

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