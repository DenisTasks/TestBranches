using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TestWpf;
using TestWpf.Administration.Groups;
using TestWpf.Administration.Users;
using TestWpf.Pages;

namespace MVVM
{
    /// <summary>
    /// Interaction logic for TestShell.xaml
    /// </summary>
    public partial class TestShell : Window
    {
        public TestShell()
        {
            InitializeComponent();
            _mainFrame.Navigate(new LoginPage());
            Messenger.Default.Register<NotificationMessage>(this, NotificationMessageReceived);
        }

        public void NotificationMessageReceived(NotificationMessage obj)
        {
            if (obj.Notification.Equals("LoginSuccess"))
            {
                _mainFrame.Navigate(new MainWindowPage());
            }
        }
    }
}