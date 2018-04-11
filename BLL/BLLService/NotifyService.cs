using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BLL.EntitesDTO;
using BLL.Interfaces;
using Model.Entities;
using Model.Interfaces;
using AutoMapper;

namespace BLL.BLLService
{
    public class NotifyService : INotifyService
    {
        private readonly IGenericRepository<Notification> _notifications;
        private readonly IGenericRepository<User> _users;
        private readonly IGenericRepository<Location> _locations;


        public NotifyService(IGenericRepository<Notification> notifications, IGenericRepository<User> users, IGenericRepository<Location> locations)
        {
            _notifications = notifications;
            _users = users;
            _locations = locations;
        }

        public IEnumerable<NotificationDTO> GetNotificationsByUserId(int id)
        {
            List<Notification> collection;
            using (_notifications.BeginTransaction())
            {
                collection = _notifications.Get(x => x.Users.Any(s => s.UserId == id)).ToList();
            }
            var mappingCollection = Mapper.Map<IEnumerable<Notification>, IEnumerable<NotificationDTO>>(collection).ToList();
            foreach (var item in mappingCollection)
            {
                item.Room = _locations.FindById(item.LocationId).Room;
            }
            return mappingCollection;
        }

        public void AddNotification(AppointmentDTO appointment, int organizerId)
        {
            var notificationItem = Mapper.Map<AppointmentDTO, Notification>(appointment);
            notificationItem.OrganizerId = organizerId;
            notificationItem.Organizer = _users.FindById(organizerId);
            notificationItem.Users = new List<User>();
            var convert = Mapper.Map<IEnumerable<UserDTO>, IEnumerable<User>>(appointment.Users);
            foreach (var item in convert)
            {
                if (_users.FindById(item.UserId) != null)
                {
                    notificationItem.Users.Add(_users.FindById(item.UserId));
                }
            }

            using (var transaction = _notifications.BeginTransaction())
            {
                try
                {
                    _notifications.Create(notificationItem);
                    _notifications.Save();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    MessageBox.Show(e + " from Notify service!");
                }
            }
        }

        public void RemoveFromNotification(int notifyId, int userId)
        {
            var notification = _notifications.FindById(notifyId);
            if (notification != null)
            {
                using (var transaction = _notifications.BeginTransaction())
                {
                    try
                    {
                         notification.Users.Remove(_users.FindById(userId));
                        _notifications.Save();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw new Exception(e + " from Notify service!");
                    }
                }
            }
        }

        public void RemoveNotification(int notifyId, int userId)
        {
            var notification = _notifications.FindById(notifyId);
            if (notification != null)
            {
                using (var transaction = _notifications.BeginTransaction())
                {
                    try
                    {
                        if (notification.OrganizerId == userId || notification.Users.Count == 1 && notification.Users.ElementAt(0).UserId == userId)
                        {
                            _notifications.Remove(notification);
                        }
                        else
                        {
                            notification.Users.Remove(_users.FindById(userId));
                        }
                        _notifications.Save();
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw new Exception(e + " from Notify service!");
                    }
                }
            }
        }
    }
}