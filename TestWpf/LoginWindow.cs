using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TestWpf.Helpers;
using TestWpf.ViewModel;
using ViewModel.Authentication;
using ViewModel.Interfaces;

namespace TestWpf
{
    public partial class LoginWindow : Window, IView
    {

        public LoginWindow(AuthenticationViewModel viewModel)
        {
            //ViewModel = viewModel;
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);

        }
        

        public IViewModel ViewModel
        {
            get { return DataContext as IViewModel; }
            set { DataContext = value; }
        }


        public void NotificationMessageReceived(NotificationMessage obj)
        {
            if (obj.Notification.Equals("LoginSuccess"))
            {
                var addAppWindowVM = SimpleIoc.Default.GetInstance<MainViewModel>();
                var addAppWindow = new MainWindow()
                {
                    DataContext = addAppWindowVM
                };
                var result = addAppWindow.ShowDialog() ?? false;
                //var test = Window.GetWindow(this);
                //test.Close();
            }
        }
    }
}
