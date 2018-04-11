using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ViewModel.Helpers;
using ViewModel.ViewModels.Authenication;

namespace ViewModel.ViewModels.Calendar
{
    public class SyncWindowViewModel: ViewModelBase
    {
        private int _myStartDay;
        private int _myFinishDay;
        private int _syncStartDay;
        private int _syncFinishDay;

        public int MyStartDay
        {
            get => _myStartDay;
            set
            {
                _myStartDay = value;
                base.RaisePropertyChanged();
            }
        }
        public int MyFinishDay
        {
            get => _myFinishDay;
            set
            {
                _myFinishDay = value;
                base.RaisePropertyChanged();
            }
        }
        public int SyncStartDay
        {
            get => _syncStartDay;
            set
            {
                _syncStartDay = value;
                base.RaisePropertyChanged();
            }
        }
        public int SyncFinishDay
        {
            get => _syncFinishDay;
            set
            {
                _syncFinishDay = value;
                base.RaisePropertyChanged();
            }
        }

        private readonly IBLLServiceMain _service;
        private readonly int _id;
        private ObservableCollection<AppointmentDTO> _appointments;
        private ObservableCollection<AppointmentDTO> _appointmentsOther;

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
        public ObservableCollection<AppointmentDTO> AppointmentsSync
        {
            get => _appointmentsOther;
            set
            {
                if (value != _appointmentsOther)
                {
                    _appointmentsOther = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public SyncWindowViewModel(IBLLServiceMain service)
        {
            _service = service;
            CustomPrincipal cp = (CustomPrincipal)Thread.CurrentPrincipal;
            if (cp != null) _id = cp.Identity.UserId;

            Messenger.Default.Register<OpenWindowMessage>(this, message =>
            {
                if (message.Type == WindowType.None && message.User != null)
                {
                    MyStartDay = Convert.ToInt32(message.Argument);
                    MyFinishDay = MyStartDay + 7;
                    SyncStartDay = MyStartDay;
                    SyncFinishDay = MyFinishDay;
                    LoadData(message.User.UserId);
                }
            });
        }

        private void LoadData(int id)
        {
            try
            {
                Appointments = new ObservableCollection<AppointmentDTO>(_service.GetAppointmentsForCalendar(_id));
                AppointmentsSync = new ObservableCollection<AppointmentDTO>(_service.GetAppointmentsForCalendar(id));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
    }
}