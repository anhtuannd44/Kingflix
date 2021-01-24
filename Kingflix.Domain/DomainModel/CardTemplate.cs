using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("CardTemplate")]
    public partial class CardTemplate
    {
        
        public CardTemplate()
        {
           
        }

        [Key]
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "Bạn chưa nhập tiêu đề thiệp")]
        public string Title { get; set; }

        [Display(Name = "Nội dung thiệp mặc định")]
        [AllowHtml]
        public string Content { get; set; }

    }
}