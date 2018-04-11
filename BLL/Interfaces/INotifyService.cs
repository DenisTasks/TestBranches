using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.EntitesDTO;

namespace BLL.Interfaces
{
    public interface INotifyService
    {
        IEnumerable<NotificationDTO> GetNotificationsByUserId(int id);
        void AddNotification(AppointmentDTO appointment, int organizerId);
        void RemoveFromNotification(int notifyId, int userId);
        void RemoveNotification(int notifyId, int userId);
    }
}