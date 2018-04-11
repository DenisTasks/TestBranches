using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using ViewModel.Helpers;
using ViewModel.Models;


namespace ViewModel.ViewModels.Appointments
{
    public class AboutAppointmentWindowViewModel : ViewModelBase
    {
        private AppointmentModel _appointment;
        private LocationDTO _location;
        private RelayCommand<Window> _printAppointmentCommand;

        public RelayCommand<Window> PrintAppointmentCommand
        {
            get => _printAppointmentCommand;
        }

        public LocationDTO Location
        {
            get => _location;
            private set
            {
                if (value != _location)
                {
                    _location = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        private void PrintAppointment(Window window)
        {
            PrintHelper.PrintAppointment(_appointment);
            window.Close();
        }

        public AppointmentModel Appointment
        {
            get => _appointment;
            private set
            {
                if (value != _appointment)
                {
                    _appointment = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public AboutAppointmentWindowViewModel(IBLLServiceMain service)
        {
            Messenger.Default.Register<OpenWindowMessage>(this, message =>
            {
                if (message.Argument == "Load this appointment")
                {
                    Appointment = message.Appointment;
                    Location = service.GetLocationById(message.Appointment.LocationId);
                    Messenger.Default.Unregister<OpenWindowMessage>(this);
                }
            });
            _printAppointmentCommand = new RelayCommand<Window>(PrintAppointment);
        }
    }

}
