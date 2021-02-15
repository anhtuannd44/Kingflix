using System.ComponentModel.DataAnnotations;

namespace Kingflix.Website.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Bạn chưa nhập Email")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu mới")]
        [StringLength(100, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận chưa đúng.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Bạn chưa nhập Email")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}