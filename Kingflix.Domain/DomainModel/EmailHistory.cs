using Kingflix.Domain.Enumerables;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kingflix.Domain.DomainModel
{
    [Table("EmailHistory")]
    public partial class EmailHistory
    {
        public EmailHistory()
        {

        }

        [Key]
        public int Id { get; set; }

        public string Email { get; set; }

        public DateTime TimeSend { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public EmailStatus Status { get; set; }

    }
}