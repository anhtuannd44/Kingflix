using Kingflix.Domain.Enumerables;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kingflix.Website.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    public class UserViewModel
    {
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng Email")]
        public string Email { get; set; }
        public string FullName { get; set; }
        [Phone(ErrorMessage = "Chưa đúng định dạng điện thoại")]
        public string PhoneNumber { get; set; }
        public Gentle Gentle { get; set; }
        public DateTime? Birthday { get; set; }
        public double FeedbackStar { get; set; }
        public string FeedbackContent { get; set; }
        public string Address { get; set; }
        public string Referral { get; set; }
        public byte[] Avatar { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu cũ")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu cũ")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "{0} phải từ {2} ký tự trở lên.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu chưa chính xác.")]
        public string ConfirmPassword { get; set; }
    }

}