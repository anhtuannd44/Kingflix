using Kingflix.Domain.Enumerables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("CategoryNetflixDetails")]
    public partial class CategoryNetflixDetails
    {
        public CategoryNetflixDetails()
        {
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mục này không được để trống")]
        [StringLength(300)]
        [Display(Name = "Hạng mục")]
        public string Name { get; set; }

        public string ContentNetflix1 { get; set; }
        public string ContentNetflix2 { get; set; }
        public string ContentNetflix3 { get; set; }
        public string ContentNetflix4 { get; set; }
        public bool? CheckNetflix1 { get; set; }
        public bool? CheckNetflix2 { get; set; }
        public bool? CheckNetflix3 { get; set; }
        public bool? CheckNetflix4 { get; set; }

        public CategoryDetailsType Type { get; set; }

        public int OrderBy { get; set; }

        [Display(Name = "Trạng thái hiển thị")]
        public bool IsShow { get; set; }

        [NotMapped]
        public bool IsShowNetFlix1 { get; set; }
        [NotMapped]
        public bool IsShowNetFlix2 { get; set; }
        [NotMapped]
        public bool IsShowNetFlix3 { get; set; }
        [NotMapped]
        public bool IsShowNetFlix4 { get; set; }
    }
}