using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using Model;
using Model.Entities;
using Model.Interfaces;

namespace BLL.BLLService
{
    public class BLLServiceMain : IBLLServiceMain
    {
        private readonly WPFOutlookContext _context;
        private readonly IGenericRepository<Appointment> _appointments;
        private readonly IGenericRepository<User> _users;
        private readonly IGenericRepository<Location> _locations;

        public BLLServiceMain(IGenericRepository<Appointment> appointments, IGenericRepository<User> users, IGenericRepository<Location> locations, WPFOutlookContext context)
        {
            //111
            //222
            //333
            //444
            //555
            //666
            //777dev
            //888dem
            //
            //
            //
            _appointments = appointments;
            _users = users;
            _locations = locations;
            _context = context;
        }

        public IEnumerable<AppointmentDTO> GetAppointmentsByUserId(int id)
        {
            List<Appointment> collection;
            using (_appointments.BeginTransaction())
            {
                collection = _appointments.Get(x => x.Users.Any(s => s.UserId == id)).ToList();
            }
            var mappingCollection = Mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDTO>>(collection).ToList();
            foreach (var item in mappingCollection)
            {
                using (_locations.BeginTransaction())
                {
                    item.Room = _locations.FindById(item.LocationId).Room;
                }
            }
            return mappingCollection;
        }

        public IEnumerable<AppointmentDTO> GetAppointmentsByUserIdSql()
        {
            List<Appointment> collection;
            using (new WPFOutlookContext())
            {
                collection = _context.Database.SqlQuery<Appointment>("GetApps").ToList();
            }
            var mappingCollection = Mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDTO>>(collection).ToList();
            foreach (var item in mappingCollection)
            {
                using (_locations.BeginTransaction())
                {
                    item.Room = _locations.FindById(item.LocationId).Room;
                }
            }
            return mappingCollection;
        }

        public IEnumerable<AppointmentDTO> GetAppointmentsForCalendar(int id)
        {
            List<Appointment> collection;
            using (_appointments.BeginTransaction())
            {
                collection = _appointments.Get(x => x.Users.Any(s => s.UserId == id)).ToList();
            }
            var mappingCollection = Mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDTO>>(collection).ToList();
            List<AppointmentDTO> forCalendar = new List<AppointmentDTO>();
            foreach (var item in mappingCollection)
            {
                using (_locations.BeginTransaction())
                {
                    item.Room = _locations.FindById(item.LocationId).Room;
                }
                if (item.BeginningDate.DayOfYear < item.EndingDate.DayOfYear)
                {
                    for (int i = 0; i <= item.EndingDate.DayOfYear - item.BeginningDate.DayOfYear; i++)
                    {
                        AppointmentDTO part = new AppointmentDTO
                        {
                            AppointmentId = item.AppointmentId,
                            LocationId = item.LocationId,
                            OrganizerId = item.OrganizerId,
                            Room = item.Room,
                            Subject = item.Subject,
                            Users = item.Users,
                        };
                        if (i == 0)
                        {
                            // first part
                            part.BeginningDate = item.BeginningDate;
                            part.EndingDate = item.BeginningDate.Date.AddDays(1).AddSeconds(-1);
                            forCalendar.Add(part);
                        }
                        if (i == item.EndingDate.DayOfYear - item.BeginningDate.DayOfYear)
                        {
                            // last part
                            part.BeginningDate = item.EndingDate.Date.AddSeconds(1);
                            part.EndingDate = item.EndingDate;
                            forCalendar.Add(part);
                        }
                        else
                        {
                            //
                            // default part
                            part.BeginningDate = item.BeginningDate.Date.AddDays(i);
                            part.EndingDate = item.BeginningDate.Date.AddDays(i + 1).AddSeconds(-1);
                            forCalendar.Add(part);
                        }
                    }
                }
                else
                {
                    forCalendar.Add(item);
                }
            }
            return forCalendar;
        }

        public LocationDTO GetLocationById(int id)
        {
            Location location;
            using (_locations.BeginTransaction())
            {
                location = _locations.FindById(id);
            }
            var mappingItem = Mapper.Map<Location, LocationDTO>(location);
            return mappingItem;
        }

        public IEnumerable<LocationDTO> GetLocations()
        {
            var locationsMapper = Mapper.Map<IEnumerable<Location>, IEnumerable<LocationDTO>>(_locations.Get());
            return locationsMapper;
        }

        public IEnumerable<AppointmentDTO> GetAppsByLocation(int id)
        {
            List<Appointment> collection;
            using (_appointments.BeginTransaction())
            {
                collection = _appointments.Get(x => x.LocationId == id).ToList();
            }
            var mappingCollection = Mapper.Map<IEnumerable<Appointment>, IEnumerable<AppointmentDTO>>(collection).ToList();
            foreach (var item in mappingCollection)
            {
                using (_locations.BeginTransaction())
                {
                    item.Room = _locations.FindById(item.LocationId).Room;
                }
            }
            return mappingCollection;
        }

        public IEnumerable<UserDTO> GetUsers()
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(_users.Get());
        }

        public IEnumerable<UserDTO> GetAppointmentUsers(int id)
        {
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(_appointments.FindById(id).Users.ToList());
        }

        public void AddAppointment(AppointmentDTO appointment, int id)
        {
            var appointmentItem = Mapper.Map<AppointmentDTO, Appointment>(appointment);
            appointmentItem.OrganizerId = id;
            appointmentItem.Organizer = _users.FindById(id);
            appointmentItem.Location = _locations.FindById(appointmentItem.LocationId);
            appointmentItem.Users = new List<User>();
            var convert = Mapper.Map<IEnumerable<UserDTO>, IEnumerable<User>>(appointment.Users);
            foreach (var item in convert)
            {
                if (_users.FindById(item.UserId) != null)
                {
                    appointmentItem.Users.Add(_users.FindById(item.UserId));
                }
            }

            using (var transaction = _appointments.BeginTransaction())
            {
                try
                {
                    _appointments.Create(appointmentItem);
                    _appointments.Save();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    MessageBox.Show(e + " from BLL service!");
                }
            }
        }

        public void RemoveFromAppointment(int appointmentId, int userId)
        {
            var appointment = _appointments.FindById(appointmentId);
            using (var transaction = _appointments.BeginTransaction())
            {
                try
                {
                    if (appointment.OrganizerId == userId || appointment.Users.Count == 1 && appointment.Users.ElementAt(0).UserId == userId)
                    {
                        _appointments.Remove(appointment);
                    }
                    else
                    {
                        appointment.Users.Remove(_users.FindById(userId));
                    }
                    _appointments.Save();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception(e + " from BLL");
                }
            }
        }

        public int OverlappingDates(DateTime beginningDate, DateTime endingDate, int locationId)
        {
            var param1 = new SqlParameter("@beginningDate", beginningDate);
            var param2 = new SqlParameter("@endingDate", endingDate);
            var param3 = new SqlParameter("@locationId", locationId);
            var returnCode = new SqlParameter
            {
                ParameterName = "@returnCode",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            using (new WPFOutlookContext())
            {
                _context.Database.ExecuteSqlCommand("OverlappingDates @beginningDate, @endingDate, @locationId, @returnCode OUTPUT", param1, param2, param3, returnCode);
                var returnCodeValue = (int)returnCode.Value;
                return returnCodeValue;
            }
        }

        public int CheckOverlappingDates(DateTime beginningDate, DateTime endingDate, int locationId)
        {
            var param1 = new SqlParameter("@beginningDate", beginningDate);
            var param2 = new SqlParameter("@endingDate", endingDate);
            var param3 = new SqlParameter("@locationId", locationId);

            List<Appointment> result;
            using (new WPFOutlookContext())
            {
                result = _context.Database.SqlQuery<Appointment>("TestDates @beginningDate, @endingDate, @locationId", param1, param2, param3).ToList();
            }

            return result.Count;
        }
    }
}