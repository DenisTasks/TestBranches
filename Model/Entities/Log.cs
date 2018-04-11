using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Entities
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }

        public string Action { get; set; }
        public string AppointmentName { get; set; }

        public int ActionAuthorId { get; set; }
        public User ActionAuthor { get; set; }

        public int CreatorId { get; set; }
        public User Creator { get; set; }

        public DateTime EventTime { get; set; }
    }
}
