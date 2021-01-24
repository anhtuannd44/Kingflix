using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("Question")]
    public partial class Question
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập tiêu đề")]
        [Display(Name = "Câu hỏi")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Bạn chưa nhập nội dung")]
        [Display(Name = "Trả lời")]
        public string Content { get; set; }

    }
}