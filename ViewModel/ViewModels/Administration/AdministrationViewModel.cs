using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;

namespace ViewModel.ViewModels.Administration
{
    public class AdministrationViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly IAdministrationService _administrationService;



        public AdministrationViewModel(IAdministrationService administrationService)
        {
            _administrationService = administrationService;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
