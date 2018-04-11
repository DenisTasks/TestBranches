using BLL.EntitesDTO;
using BLL.Interfaces;
using Model.Entities;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.BLLService
{
    public class LogService : ILogService
    {

        private readonly IGenericRepository<Log> _logs;

        public LogService(IGenericRepository<Log> logs)
        {
            _logs = logs;
        }

        public void LogAppointment(AppointmentDTO appointment, int id, bool action)
        {
            using (var transaction = _logs.BeginTransaction())
            {
                try
                {
                    _logs.Create(new Log
                    {
                        AppointmentName = appointment.Subject,
                        ActionAuthorId = id,
                        CreatorId = id,
                        Action = action ? "Added" : "Removed" ,
                        EventTime = DateTime.Now
                    });
                    _logs.Save();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e + " from BLL");
                }
            }
        }

        
    }
}
