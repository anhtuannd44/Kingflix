using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("EmailTemplate")]
    public partial class EmailTemplate
    {
        public EmailTemplate()
        {
           
        }

        [Key]
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Bạn chưa nhập tiêu đề Email")]
        public string Title { get; set; }

        [Display(Name = "Số tiền khách đã thanh toán")]
        [AllowHtml]
        public string Content { get; set; }

    }
}