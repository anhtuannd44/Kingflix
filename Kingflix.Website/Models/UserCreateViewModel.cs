using Kingflix.Domain.Enumerables;
using System.ComponentModel.DataAnnotations;

namespace Kingflix.Website.Models
{
    public class UserCreateViewModel
    {

        [Required(ErrorMessage = "Bạn chưa nhập Email")]
        [EmailAddress(ErrorMessage = "Định dạng Email chưa đúng")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập mật khẩu")]
        [StringLength(100, ErrorMessage = "{0} phải từ {2} ký tự trở lên.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Chưa đúng định dạng số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Bạn chưa chọn giới tính")]
        [Display(Name = "Giới tính")]
        public Gentle Gentle { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Họ và Tên")]
        [RegularExpression(@"^[aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆfFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTuUùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ ]*$", ErrorMessage = "Họ và tên chỉ được nhập chữ")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

    }
}