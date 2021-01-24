using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Kingflix.Domain.DomainModel
{
    [Table("Profile")]
    public partial class Profile
    {

        public Profile()
        {
            EntendProfiles = new HashSet<ExtendProfile>();
        }

        [Key]
        public int ProfileId { get; set; }

        [StringLength(300)]
        [Display(Name = "Tên Profile")]
        public string Name { get; set; }

        [Display(Name = "Mã PIN")]
        public string Pincode { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateCreated { get; set; }

        public DateTime? DateModified { get; set; }

        [Display(Name = "Tài khoản")]
        public string ProductId { get; set; } //ForeignKey

        public string UserId { get; set; } //ForeignKey

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public string Note { get; set; }
        public string OrderId { get; set; }

        [ForeignKey("UserId")]
        public virtual AppUser UserInformation { get; set; }
        public virtual Product Products { get; set; }
        public virtual Order Orders { get; set; }
        public ICollection<ExtendProfile> EntendProfiles { get; set; }


        //Data show to view
        [NotMapped]
        public bool SlotCheck = false;
        [NotMapped]
        public UserProductStatusShow StatusShow
        {
            get
            {
                if (DateEnd.Subtract(DateTime.Now).Days > 10)
                    return UserProductStatusShow.Using;

                else if (DateEnd.Subtract(DateTime.Now).Days < 0)
                    return UserProductStatusShow.Expired;
                else
                    return UserProductStatusShow.PreExpired;
            }
        }
        [NotMapped]
        public bool IsError { get; set; }
    }
}