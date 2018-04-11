using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ViewModel.Helpers;
using ViewModel.Models;

namespace ViewModel.ViewModels.Appointments
{
    public class AllAppByLocationWindowViewModel : ViewModelBase
    {
        private ObservableCollection<AppointmentModel> _appointments;

        public ObservableCollection<AppointmentModel> Appointments
        {
            get => _appointments;
            private set
            {
                if (value != _appointments)
                {
                    _appointments = value;
                    base.RaisePropertyChanged();
                }
            }
        }

        public AllAppByLocationWindowViewModel(IBLLServiceMain service)
        {
            Messenger.Default.Register<OpenWindowMessage>(this, message =>
            {
                if (message.Type == WindowType.LoadLocations && message.Argument != null)
                {
                    Appointments = new ObservableCollection<AppointmentModel>(Mapper.Map<IEnumerable<AppointmentDTO>, ICollection<AppointmentModel>>(service.GetAppsByLocation(Int32.Parse(message.Argument))));
                }
            });
        }
    }
}