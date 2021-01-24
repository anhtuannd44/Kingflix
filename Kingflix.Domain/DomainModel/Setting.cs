using Kingflix.Domain.Enumerables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("Setting")]
    public partial class Setting
    {
        public Setting()
        {
           
        }

        [Key]
        public string SettingId { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Nội dung")]
        [Display(Name = "Nội dung")]
        [AllowHtml]
        public string Content { get; set; }

        public bool? IsDownPage { get; set; }

        public Status Status { get; set; }
    }
}