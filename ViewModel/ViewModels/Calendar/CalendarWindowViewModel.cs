using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using ViewModel.Helpers;
using ViewModel.ViewModels.Authenication;

namespace ViewModel.ViewModels.Calendar
{
    public class CalendarWindowViewModel : ViewModelBase
    {
        private readonly IBLLServiceMain _service;
        private readonly int _id;
        private ObservableCollection<AppointmentDTO> _appointments;
        private ObservableCollection<UserDTO> _users;
        private UserDTO _selectedSyncUser;

        public ObservableCollection<UserDTO> Users
        {
            get => _users;
            set
            {
                if (value != _users)
                {
                    _users = value;
                    base.RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<AppointmentDTO> Appointments
        {
            get => _appointments;
            set
            {
                if (value != _appointments)
                {
                    _appointments = value;
                    base.RaisePropertyChanged();
                }
            }
        }
        public UserDTO SelectedSyncUser
        {
            get => _selectedSyncUser;
            set
            {
                if (value != _selectedSyncUser)
                {
                    _selectedSyncUser = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public RelayCommand<object> SyncCommand { get; }

        public CalendarWindowViewModel(IBLLServiceMain service)
        {
            _service = service;
            CustomPrincipal cp = (CustomPrincipal)Thread.CurrentPrincipal;
            if (cp != null) _id = cp.Identity.UserId;
            LoadData();
            SyncCommand = new RelayCommand<object>(SyncWithUser);
        }

        private void SyncWithUser(object start)
        {
            if (_selectedSyncUser != null)
            {
                Messenger.Default.Send(new OpenWindowMessage { Type = WindowType.Sync, User = new UserDTO { UserId = _selectedSyncUser.UserId }, Argument = start.ToString()});
            }
        }
        private void LoadData()
        {
            try
            {
                Appointments = new ObservableCollection<AppointmentDTO>(_service.GetAppointmentsForCalendar(_id));
                Users = new ObservableCollection<UserDTO>(_service.GetUsers());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}