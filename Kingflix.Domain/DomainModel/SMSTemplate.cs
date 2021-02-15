using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("SMSTemplate")]
    public partial class SMSTemplate
    {
        public SMSTemplate()
        {
           
        }

        [Key]
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Bạn chưa nhập tiêu đề ")]
        public string Title { get; set; }

        [Display(Name = "Nội dung")]
        public string Content { get; set; }

    }
}