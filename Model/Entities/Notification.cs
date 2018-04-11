using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Model.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string Subject { get; set; }
        public DateTime BeginningDate { get; set; }
        public DateTime EndingDate { get; set; }

        public int OrganizerId { get; set; }
        public User Organizer { get; set; }

        public int LocationId { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}