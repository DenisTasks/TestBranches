using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Models;

namespace ViewModel.ViewModels.Administration
{
    public class ShowAllLogsViewModel: ViewModelBase
    {
        private ObservableCollection<LogModel> _logs;
        private IAdministrationService _administrationService;

        public ObservableCollection<LogModel> Logs
        {
            get => _logs;
            set
            {
                _logs = value;
                base.RaisePropertyChanged();
            }
        }

        public ShowAllLogsViewModel(IAdministrationService administrationService)
        {
            _administrationService = administrationService;
            Logs = new ObservableCollection<LogModel>(Mapper.Map<IEnumerable<LogDTO>, ICollection<LogModel>>(_administrationService.GetLogs()));
        }
    }
}