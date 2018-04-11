using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ViewModel.Models;
using ViewModel.ViewModels.Authenication;

namespace ViewModel.ViewModels.Appointments
{
    public class AddAppWindowViewModel : ViewModelBase
    {
        private readonly IBLLServiceMain _service;
        private readonly INotifyService _notifyService;
        private readonly ILogService _logService;

        private ObservableCollection<UserDTO> _userList;
        private ObservableCollection<UserDTO> _selectedUserList;
        private ObservableCollection<AppointmentModel> _templateApps;

        private DateTime _parseStartDate;
        private DateTime _parseEndingDate;
        private DateTime _selectedBeginningTime;
        private DateTime _selectedEndingTime;
        private DateTime _startDate = DateTime.Today;
        private DateTime _endingDate = DateTime.Today;
        private LocationDTO _selectedLocation;
        private AppointmentModel _selectedTemplateItem;
        private bool _allDayEvent;
        //private int _isAvailible;
        private int Id { get; }

        public RelayCommand<Window> CreateAppCommand { get; }
        public RelayCommand<UserDTO> AddUserToListCommand { get; }
        public RelayCommand<UserDTO> RemoveUserFromListCommand { get; }

        public ObservableCollection<UserDTO> SelectedUserList
        {
            get => _selectedUserList;
            set
            {
                if (value != _selectedUserList)
                {
                    _selectedUserList = value;
                    base.RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<UserDTO> UserList
        {
            get => _userList;
            private set
            {
                if (value != _userList)
                {
                    _userList = value;
                    base.RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<AppointmentModel> TemplateApps
        {
            get => _templateApps;
            private set
            {
                if (value != _templateApps)
                {
                    _templateApps = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public AppointmentModel Appointment { get; set; }

        // combo boxes
        public List<DateTime> BeginningTime { get; }
        public List<DateTime> EndingTime { get; }
        public List<LocationDTO> LocationList { get; }

        // selected
        public AppointmentModel SelectedTemplateItem
        {
            get => _selectedTemplateItem;
            set
            {
                _selectedTemplateItem = value;
                Appointment.Subject = value.Subject;
                SelectedLocation = LocationList.First(x => x.LocationId == value.LocationId);
                StartBeginningDate = value.BeginningDate;
                EndBeginningDate = value.EndingDate;
                SelectedBeginningTime = BeginningTime.First(x =>
                    x.Hour == value.BeginningDate.Hour && x.Minute == value.BeginningDate.Minute);
                SelectedEndingTime = BeginningTime.First(x =>
                    x.Hour == value.EndingDate.Hour && x.Minute == value.EndingDate.Minute);
                base.RaisePropertyChanged();
            }
        }
        public LocationDTO SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                base.RaisePropertyChanged();
            }
        }
        public DateTime StartBeginningDate
        {
            get => _startDate;
            set
            {
                if (EndBeginningDate != value)
                {
                    EndBeginningDate = value;
                }
                _startDate = value;
                base.RaisePropertyChanged();
            }
        }
        public DateTime EndBeginningDate
        {
            get => _endingDate;
            set
            {
                _endingDate = value;
                base.RaisePropertyChanged();
            }
        }
        public DateTime SelectedBeginningTime
        {
            get => _selectedBeginningTime;
            set
            {
                _selectedBeginningTime = value;
                base.RaisePropertyChanged();
            }
        }
        public DateTime SelectedEndingTime
        {
            get => _selectedEndingTime;
            set
            {
                _selectedEndingTime = value;
                base.RaisePropertyChanged();
            }
        }
        public bool AllDayEvent
        {
            get => _allDayEvent;
            set
            {
                _allDayEvent = value;
                base.RaisePropertyChanged();
            }
        }

        public AddAppWindowViewModel(IBLLServiceMain service, ILogService logService, INotifyService notifyService)
        {
            _service = service;
            _logService = logService;
            _notifyService = notifyService;

            AddUserToListCommand = new RelayCommand<UserDTO>(AddUsersToList);
            RemoveUserFromListCommand = new RelayCommand<UserDTO>(RemoveUsersFromList);
            CreateAppCommand = new RelayCommand<Window>(CreateAppointment);

            UserList = new ObservableCollection<UserDTO>(_service.GetUsers());
            SelectedUserList = new ObservableCollection<UserDTO>();
            LocationList = _service.GetLocations().ToList();

            CustomPrincipal cp = Thread.CurrentPrincipal as CustomPrincipal;
            if (cp != null) Id = cp.Identity.UserId;
            TemplateApps = new ObservableCollection<AppointmentModel>(Mapper.Map<IEnumerable<AppointmentDTO>, ICollection<AppointmentModel>>(_service.GetAppointmentsByUserId(Id)));

            Appointment = new AppointmentModel();

            BeginningTime = LoadTimeRange();
            EndingTime = LoadTimeRange();
            SelectedBeginningTime = BeginningTime.Find(x => x == GetDateTimeNow());
            SelectedEndingTime = BeginningTime[BeginningTime.IndexOf(SelectedBeginningTime) + 1];
            SelectedLocation = LocationList[0];

            AllDayEvent = false;

            AddUsersToList(UserList.FirstOrDefault(s => s.UserId == Id));
        }

        public int GetLocationsCount()
        {
            return LocationList.Count;
        }

        public int GetUsersCount()
        {
            return UserList.Count;
        }

        public DateTime GetDateTimeNow()
        {
            var dateTimeNow = DateTime.Now.Ticks;
            var checkTime = new DateTime();
            foreach (var item in BeginningTime)
            {
                if (dateTimeNow > item.Ticks)
                {
                    int nextTime = BeginningTime.IndexOf(item) + 1;
                    checkTime = nextTime <= BeginningTime.Count ? BeginningTime.ElementAt(nextTime) : BeginningTime[0];
                }
            }
            return checkTime;
        }

        private List<DateTime> LoadTimeRange()
        {
            var timeList = new List<DateTime>();

            DateTime day = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);
            DateTime day2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 00);
            for (TimeSpan i = day.TimeOfDay; i < day2.TimeOfDay; i += TimeSpan.FromMinutes(30))
            {
                timeList.Add(DateTime.Parse(i.ToString()));
            }
            return timeList;
        }

        //private void CheckDates()
        //{
        //    _isAvailible = 0;
        //    if (_selectedLocation != null)
        //    {
        //        var bySameLocation = _service.GetAppsByLocation(_selectedLocation.LocationId).ToList();

        //        foreach (var b in bySameLocation)
        //        {
        //            bool overlap = _parseStartDate < b.EndingDate && b.BeginningDate < _parseEndingDate;
        //            if (overlap)
        //            {
        //                _isAvailible++;
        //            }
        //        }
        //    }
        //}

        private void CreateAppointment(Window window)
        {
            string startDate;
            string endingDate;
            if (AllDayEvent)
            {
                startDate = _startDate.ToString(CultureInfo.InvariantCulture);
                endingDate = _startDate.AddDays(1).AddSeconds(-1).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                startDate = $"{_startDate.ToString("d", CultureInfo.InvariantCulture)} {_selectedBeginningTime.ToString("h:mm tt", CultureInfo.InvariantCulture)}";
                endingDate = $"{_endingDate.ToString("d", CultureInfo.InvariantCulture)} {_selectedEndingTime.ToString("h:mm tt", CultureInfo.InvariantCulture)}";
            }
            _parseStartDate = DateTime.Parse(startDate, CultureInfo.InvariantCulture);
            _parseEndingDate = DateTime.Parse(endingDate, CultureInfo.InvariantCulture);

            Appointment.BeginningDate = _parseStartDate;
            Appointment.EndingDate = _parseEndingDate;
            Appointment.LocationId = SelectedLocation.LocationId;
            Appointment.Room = SelectedLocation.Room;
            Appointment.Users = SelectedUserList;
            //CheckDates();

            var overlappingDates = _service.CheckOverlappingDates(_parseStartDate, _parseEndingDate, Appointment.LocationId);

            if (Appointment.IsValid && overlappingDates == 0)
            {
                _logService.LogAppointment(Mapper.Map<AppointmentModel, AppointmentDTO>(Appointment), Id, true);
                _service.AddAppointment(Mapper.Map<AppointmentModel, AppointmentDTO>(Appointment), Id);
                _notifyService.AddNotification(Mapper.Map<AppointmentModel, AppointmentDTO>(Appointment), Id);

                window.Close();
                Messenger.Default.Send<NotificationMessage, MainWindowViewModel>(new NotificationMessage("Refresh"));
            }
            else
            {
                MessageBox.Show(SelectedUserList.Count == 0
                    ? "Please add users to the appointment list"
                    : $"The selected room is busy for the specified period of time! You have {overlappingDates} intersections!");
            }
        }

        private void AddUsersToList(UserDTO user)
        {
            if (user != null)
            {
                SelectedUserList.Add(user);
                UserList.Remove(user);
                base.RaisePropertyChanged();
            }
        }

        private void RemoveUsersFromList(UserDTO user)
        {
            if (user != null)
            {
                UserList.Add(user);
                SelectedUserList.Remove(user);
                base.RaisePropertyChanged();
            }
        }
    }
}