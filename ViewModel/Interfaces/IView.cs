using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Interfaces;

namespace ViewModel.Interfaces
{
    public interface IView
    {
        IViewModel ViewModel { get; set; }
        void Show();
        void NotificationMessageReceived(NotificationMessage obj);
    }
}
