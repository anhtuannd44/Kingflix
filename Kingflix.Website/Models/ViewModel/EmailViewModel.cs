using Kingflix.Domain.DomainModel;
using System.Collections.Generic;
using Kingflix.Domain.Enumerables;

namespace Kingflix.Models.ViewModel
{
    public class EmailViewModel
    {
        public EmailType Type { get; set; }
        public string[] EmailList { get; set; }
        public int EmailTemplateId { get; set; }
    }
}