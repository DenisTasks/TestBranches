using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities
{

    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }
        public string Subject { get; set; }
        public DateTime BeginningDate { get; set; }
        public DateTime EndingDate { get; set; }

        public int OrganizerId { get; set; }
        public User Organizer { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }
        
        public virtual ICollection<User> Users { get; set; }
    }
}
