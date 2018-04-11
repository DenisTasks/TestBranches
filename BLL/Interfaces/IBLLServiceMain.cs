using System;
using System.Collections.Generic;
using BLL.EntitesDTO;

namespace BLL.Interfaces
{
    public interface IBLLServiceMain
    {
        IEnumerable<AppointmentDTO> GetAppointmentsByUserId(int id);
        IEnumerable<AppointmentDTO> GetAppointmentsForCalendar(int id);
        IEnumerable<LocationDTO> GetLocations();
        IEnumerable<UserDTO> GetUsers();
        IEnumerable<AppointmentDTO> GetAppsByLocation(int id);
        IEnumerable<UserDTO> GetAppointmentUsers(int id);
        LocationDTO GetLocationById(int id);
        void AddAppointment(AppointmentDTO appointment, int id);
        void RemoveFromAppointment(int appointmentId, int userId);
        int OverlappingDates(DateTime beginningDate, DateTime endingDate, int locationId);
        int CheckOverlappingDates(DateTime beginningDate, DateTime endingDate, int locationId);
        IEnumerable<AppointmentDTO> GetAppointmentsByUserIdSql(int id);
    }
}
