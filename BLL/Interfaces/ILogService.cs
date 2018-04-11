using BLL.EntitesDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ILogService
    {
        void LogAppointment(AppointmentDTO appointment, int id, bool action);
    }
}
