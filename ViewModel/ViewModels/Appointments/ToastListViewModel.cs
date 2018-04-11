using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ViewModel.Helpers;

namespace ViewModel.ViewModels.Appointments
{
    public class ToastListViewModel : ViewModelBase
    {
        public ObservableCollection<Message4ListBox> Messages { get; set; }
        public ToastListViewModel()
        {
            Messages = new ObservableCollection<Message4ListBox>();
            Messenger.Default.Register<OpenWindowMessage>(this, message =>
            {
                if (message.Type == WindowType.Toast)
                {
                    AddMessage(message);
                }
            });

        }

        private async Task AddMessage(OpenWindowMessage msg)
        {
            Message4ListBox message4ListBox = new Message4ListBox { Msg = msg.Argument, IsGoing = false };
            Messages.Insert(0, message4ListBox);
            await Task.Delay(new TimeSpan(0, 0, msg.SecondsToShow));
            // You can't animate on removal event since there's nothing there to animate.
            // Therefore, a datatrigger is used to drive the removal animation.
            message4ListBox.IsGoing = true;
            await Task.Delay(new TimeSpan(0, 0, 0, 1, 300));
            Messages.Remove(message4ListBox);
        }
    }

    public class Message4ListBox : ViewModelBase
    {
        public String Msg { get; set; }
        private bool _isGoing;

        public bool IsGoing
        {
            get { return _isGoing; }
            set
            {
                _isGoing = value;
                RaisePropertyChanged();
            }
        }
    }
}