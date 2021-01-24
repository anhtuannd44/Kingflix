using System.ComponentModel.DataAnnotations;

namespace Kingflix.Website.Models
{
    public class UserLoginViewModel
    {
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}