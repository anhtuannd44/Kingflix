using Kingflix.Domain.Enumerables;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("Homepage")]
    public partial class Homepage
    {
        public Homepage()
        {

        }

        [Key]
        public int Id { get; set; }
        public PictureHomeType Type { get; set; }
        public string Title { get; set; }
        public string TitleColor { get; set; }
        public string Content { get; set; } //Href if Slider Category
        public string ContentColor { get; set; }
        public string SubContent { get; set; }
        public string SubContentColor { get; set; }
        public string Link { get; set; }
        public string YoutubeId { get; set; }
        public string ImageId { get; set; }
        public string BackgroundColor { get; set; }
        public string TextFixed { get; set; }
        public int OrderBy { get; set; }
        public virtual Image Image { get; set; }
    }
}