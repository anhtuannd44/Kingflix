using Kingflix.Domain.Enumerables;
using System.ComponentModel.DataAnnotations;

namespace Kingflix.Website.Models
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "Bạn chưa nhập Email")]
        [EmailAddress(ErrorMessage = "Chưa đúng định dạng Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        public string Password { get; set; }
    }
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Bạn chưa nhập Email")]
        [EmailAddress(ErrorMessage = "Định dạng Email chưa đúng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Chưa đúng định dạng số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Họ và Tên")]
        [RegularExpression(@"^[aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆfFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTuUùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ ]*$", ErrorMessage = "Họ và tên chỉ được nhập chữ")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }
    }
}