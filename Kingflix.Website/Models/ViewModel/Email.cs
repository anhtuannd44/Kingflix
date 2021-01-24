﻿using Kingflix.Domain.DomainModel;
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
    public class EmailSendViewModel
    {
        public string Email { get; set; }
        public List<Profile> Profile = new List<Profile>();
    }
}