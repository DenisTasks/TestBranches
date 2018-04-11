using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.EntitesDTO
{
    public class LogDTO
    {
        public int LogId { get; set; }
        public string Action { get; set; }
        public string AppointmentName { get; set; }
        public int ActionAuthorId { get; set; }
        public int CreatorId { get; set; }
        public DateTime EventTime { get; set; }

        public string ActionAuthorName { get; set; }
        public string CreatorName { get; set; }
    }
}
