using Kingflix.Domain.Enumerables;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Kingflix.Domain.DomainModel
{
    [Table("GoogleForm")]
    public partial class GoogleForm
    {
        public GoogleForm()
        {
           
        }

        [Key]
        public int Id { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập Link nhúng google Form")]
        [Display(Name = "Link nhúng Google Form")]
        [AllowHtml]
        public string Url { get; set; }

        public DateTime DateCreated { get; set; }

        public Status Status { get; set; }
    }
}