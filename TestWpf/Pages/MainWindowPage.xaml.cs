using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using TestWpf.Appointments;
using TestWpf.Calendar;
using TestWpf.Common.Groups;
using ViewModel.Helpers;
using ViewModel.ViewModels.Authenication;

namespace TestWpf.Pages
{
    public partial class MainWindowPage : Page
    {
        private readonly DispatcherTimer _timer;

        public MainWindowPage()
        {
            InitializeComponent();

            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += TimerOnTick;

            BackgroundWorker workerTimer = new BackgroundWorker();
            workerTimer.WorkerSupportsCancellation = true;
            workerTimer.DoWork += workerTimer_DoWork;
            workerTimer.RunWorkerAsync();

            Messenger.Default.Register<OpenWindowMessage>(
                this,
                message => {
                    if (message.Type == WindowType.AddAppWindow)
                    {
                        var addAppWindow = new AddAppWindow();
                        addAppWindow.ShowDialog();
                    }
                    if (message.Type == WindowType.AddAboutAppointmentWindow && message.Argument == "Load this appointment")
                    {
                        var addAboutWindow = new AboutAppWindow();
                        // send initialize information after create, but before show window!
                        // send this message => initialize new check at this cycle
                        Messenger.Default.Send(new OpenWindowMessage {Type = WindowType.None, Argument = message.Argument, Appointment = message.Appointment });
                        addAboutWindow.ShowDialog();
                    }
                    if (message.Type == WindowType.AddAllAppByLocationWindow)
                    {
                        var addAllAppWindow = new AllAppByLocation();
                        // send initialize information after create, but before show window!
                        Messenger.Default.Send(new OpenWindowMessage { Type = WindowType.LoadLocations, Argument = message.Argument });
                        addAllAppWindow.ShowDialog();
                    }
                    if (message.Type == WindowType.Sync && message.User != null)
                    {
                        var addSync = new SyncWindow();
                        Messenger.Default.Send(new OpenWindowMessage { Type = WindowType.None, User = message.User, Argument = message.Argument });
                        addSync.ShowDialog();
                    }
                    if (message.Type == WindowType.CalendarFrame)
                    {
                        var calendarFrameWindow = new CalendarFrame();
                        calendarFrameWindow.ShowDialog();
                    }
                });
            Messenger.Default.Register<NotificationMessage>(this, message =>
            {
                if (message.Notification.Equals("CreateGroup"))
                {
                    var addGroupWindow = new AddGroupWindow();
                    addGroupWindow.ShowDialog();
                }
            });
        }

        private void ButtonBase_Click_ToAdmin(object sender, RoutedEventArgs e)
        {
            try
            {
                _timer?.Stop();
                this.NavigationService.Navigate(new AdminPage());
            }
            catch (System.Security.SecurityException)
            {
                MessageBox.Show("You have no rights to acces this menu" );
            }
            catch (Exception)
            {
                //
                //MessageBox.Show(ex.ToString());
            }
        }

        private void ButtonBase_Click_ToLoginPage(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister<OpenWindowMessage>(this);
            Messenger.Default.Unregister<NotificationMessage>(this);
            _timer?.Stop();
            CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
            customPrincipal.Identity = new AnonymousIdentity();
            Messenger.Default.Send(new NotificationMessage("LogOut"));
            this.NavigationService.GoBack();
        }

        private void workerTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            _timer?.Start();
        }

        private void TimerOnTick(object sender, EventArgs e)
        {
            TestControl.CurrentTime = Convert.ToDateTime(DateTime.Now);
        }
    }

    public class AdminVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isAdmin = Thread.CurrentPrincipal.IsInRole("admin");

            return isAdmin ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}