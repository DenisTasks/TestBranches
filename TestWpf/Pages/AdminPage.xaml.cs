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
using TestWpf.Administration;
using TestWpf.Administration.Groups;
using TestWpf.Administration.Users;

namespace TestWpf.Pages
{
    /// <summary>
    /// Interaction logic for Page2.xaml
    /// </summary>
    [PrincipalPermission(SecurityAction.Demand, Role = "admin")]
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void Button_Click_ToLogs(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ShowAllLogsPage());
        }

        private void Button_Click_ToUsers(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ShowAllUsersPage());
        }

        private void Button_Click_ToGroups(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new ShowAllGroupsPage());
            this.NavigationService.Navigate(new ShowAllGroupsPageTest());
        }

        private void Button_Click_GoBack(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
