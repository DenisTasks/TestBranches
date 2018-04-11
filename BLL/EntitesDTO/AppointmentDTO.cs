using System;
using System.Collections.Generic;

namespace BLL.EntitesDTO
{
    public class AppointmentDTO
    {
        public int AppointmentId { get; set; }
        public string Subject { get; set; }
        public DateTime BeginningDate { get; set; }
        public DateTime EndingDate { get; set; }
        public int OrganizerId { get; set; }
        public int LocationId { get; set; }
        public string Room { get; set; }
        public ICollection<UserDTO> Users { get; set; }
    }
}