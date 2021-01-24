using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Kingflix.Domain.Enumerables;
using Kingflix.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kingflix.Domain.DomainModel.IdentityModel
{
    public class AppUser : IdentityUser
    {
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }
        [Display(Name = "Giới tính")]
        public Gentle Gentle { get; set; }
        [Display(Name = "Ngày sinh")]
        public DateTime? Birthday { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "Chỉnh sửa lần cuối")]
        public DateTime? DateModified { get; set; }
        public double FeedbackStar { get; set; }
        public string FeedbackContent { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReferralInt { get; set; }
        public double PercentForReferral { get; set; }
        public bool IsUseReferral { get; set; }
        public double KinCoin { get; set; }
        public DateTime? DatePayment { get; set; }
        public DateTime? TimeLastLogin { get; set; }
        public DateTime TimeStep2 { get; set; }
        public byte[] Avatar { get; set; }
        [NotMapped]
        public bool IsSelected { get; set; } = false;

        [NotMapped]
        public string ReferralCode
        {
            get
            {
                return ReferralHelper.GenerateReferralCode(Email, ReferralInt);
            }
        }

        [NotMapped]
        public MemberShip MemberShip { get; }

        [NotMapped]
        public bool IsAdmin { get; }
        //{
        //    get
        //    {
        //        using (var db = new ApplicationDbContext())
        //        {
        //            var UserManager = new UserManager<AppUser>(new UserStore<AppUser>(db));
        //            var role = UserManager.GetRoles(Id);
        //            foreach (var item in role)
        //            {
        //                if (item == "Admin")
        //                    return true;
        //            }
        //            return false;
        //        }
        //    }
        //}

        [NotMapped]
        public int ReferralCount
        {
            get
            {
                return Orders.AsEnumerable().Where(a => a.VoucherId == ReferralCode && a.Status == OrderStatus.Done).Count();
            }
        }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<KingCoin> KingCoins { get; set; }
        public virtual ICollection<Profile> Profiles { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Support> Supports { get; set; }

        public AppUser()
        {
            Orders = new HashSet<Order>();
            KingCoins = new HashSet<KingCoin>();
            Profiles = new HashSet<Profile>();
            Reviews = new HashSet<Review>();
            Supports = new HashSet<Support>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            ClaimsIdentity userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}
