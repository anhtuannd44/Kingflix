using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Domain.Enumerables;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("SMSHistory")]
    public partial class SMSHistory
    {
        public SMSHistory()
        {

        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public DateTime TimeSend { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public SMSStatus Status { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser UserInformation { get; set; }

    }
}