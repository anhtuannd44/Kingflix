using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("ExtendProfile")]
    public partial class ExtendProfile
    {
        
        public ExtendProfile()
        {
        }

        [Key]
        public int ExtendProfileId { get; set; }

        [Display(Name = "Profile")]
        public int ProfileId { get; set; }
        
        public string OrderId { get; set; }

        public virtual Order Orders  { get; set; }

       public virtual Profile Profile { get; set; }
    }
}