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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestWpf.Common.Groups;
using ViewModel.Models;
using ViewModel.ViewModels.Administration.Groups;

namespace TestWpf.Administration.Groups
{
    /// <summary>
    /// Interaction logic for ShowAllGroupsPageTest.xaml
    /// </summary>
    public partial class ShowAllGroupsPageTest : Page
    {
        public ShowAllGroupsPageTest()
        {
            InitializeComponent();
            Messenger.Default.Register<NotificationMessage>(this, e =>
            {
                if (e.Notification == "AddGroupWindow")
                {
                    var addGroupWindow = new AddGroupWindow();
                    var result = addGroupWindow.ShowDialog();
                }
            });
            Messenger.Default.Register<GroupModel>(this, group =>
            {
                if (group != null)
                {
                    var editUserWindow = new EditGroupWindow();
                    Messenger.Default.Send<GroupModel, EditGroupViewModel>(group);
                    var result = editUserWindow.ShowDialog();
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this);
            Messenger.Default.Unregister<GroupModel>(this);
            this.NavigationService.GoBack();
        }
    }
}
