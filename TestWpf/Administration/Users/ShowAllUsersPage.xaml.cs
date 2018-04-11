using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel.Helpers;
using ViewModel.Models;
using ViewModel.ViewModels.Administration.Users;

namespace TestWpf.Administration.Users
{
    /// <summary>
    /// Interaction logic for ShowAllUsersPage.xaml
    /// </summary>
    [PrincipalPermission(SecurityAction.Demand, Role = "admin")]
    public partial class ShowAllUsersPage : Page
    {
        public ShowAllUsersPage()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, e =>
            {
                if(e.Notification == "AddUserWindow")
                {
                    var addUserWindow = new AddUserWindow();
                    var result = addUserWindow.ShowDialog();
                }
            });
            Messenger.Default.Register<UserModel>(this, user =>
            {
                if (user != null)
                {
                    var editUserWindow = new EditUserWindow();
                    Messenger.Default.Send<UserModel, EditUserViewModel>(user);
                    var result = editUserWindow.ShowDialog();
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<UserModel>(this);
            this.NavigationService.GoBack();
        }
    }
}
