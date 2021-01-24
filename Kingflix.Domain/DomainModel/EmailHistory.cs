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

        public DateTime DateSend { get; set; }

        public EmailTypeHistory Type { get; set; }

    }
}