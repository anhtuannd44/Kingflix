using Kingflix.Domain.Enumerables;
using System;

namespace Kingflix.Models.ViewModel
{
    public class NotificationViewModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public NotificationType Type { get; set; }
        public DateTime DateCreated { get; set; }
        public bool IsRead { get; set; }
        public string Link { get; set; }
    }

}