using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ViewModel.ViewModels.Authenication
{
    public class AuthenticationViewModel : ViewModelBase,  INotifyPropertyChanged
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly RelayCommand<object> _loginCommand;
        private string _username;
        private string _status;
        private int _progressBarValue;
        private string _progressText;
        private Visibility _progressVisibility;

        public string ProgressText
        {
            get => _progressText;
            set { _progressText = value; NotifyPropertyChanged("ProgressText"); }
        }
        public int ProgressBarValue
        {
            get => _progressBarValue;
            set { _progressBarValue = value; NotifyPropertyChanged("ProgressBarValue"); }
        }
        public Visibility ProgressVisibility
        {
            get => _progressVisibility;
            set { _progressVisibility = value; NotifyPropertyChanged("ProgressVisibility"); }
        }

        public AuthenticationViewModel(IAuthenticationService authenticationService)
        {
            Messenger.Default.Register<NotificationMessage>(this, e =>
            {
                if (e.Notification.Equals("LogOut"))
                {
                    ProgressVisibility = Visibility.Hidden;
                    NotifyPropertyChanged("IsAuthenticated");
                    NotifyPropertyChanged("AuthenticatedUser");
                }
            });
            _authenticationService = authenticationService;
            _loginCommand = new RelayCommand<object>(Login, CanLogin);
            Username = "admin";
            ProgressVisibility = Visibility.Hidden;
            ProgressBarValue = 0;
            ProgressText = "Please, waiting...";
        }

        public RelayCommand<object> LoginCommand { get { return _loginCommand; } }

        public string Username
        {
            get { return _username; }
            set { _username = value; NotifyPropertyChanged("Username"); }
        }

        public string Status
        {
            get { return _status; }
            set { _status = value; NotifyPropertyChanged("Status"); }
        }

        public string AuthenticatedUser
        {
            get
            {
                if (IsAuthenticated)
                {
                    Messenger.Default.Send(new NotificationMessage("LoginSuccess"));
                    return string.Format("Signed in as {0}", Thread.CurrentPrincipal.Identity.Name);

                }
                else return "You are not authenticated!";
            }
        }

        private void workerLogin_DoWork(object sender, DoWorkEventArgs e)
        {
            PasswordBox passwordBox = e.Argument as PasswordBox;
            string clearTextPassword = passwordBox.Password;
            try
            {
                UserDTO user = _authenticationService.AuthenticateUser(Username, clearTextPassword);
                CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                if (customPrincipal == null)
                    throw new ArgumentException("The application's default thread principal must be set to a CustomPrincipal object on startup.");
                customPrincipal.Identity = new CustomIdentity(user.UserId, user.UserName, user.Name, _authenticationService.GetRoles(user.UserId));
                _loginCommand.RaiseCanExecuteChanged();
                Status = String.Empty;
                NotifyPropertyChanged("Status");
                NotifyPropertyChanged("IsAuthenticated");
                ProgressBarValue = 100;
                ProgressText = "Login success!";
                Thread.Sleep(100);
                NotifyPropertyChanged("AuthenticatedUser");
            }
            catch (UnauthorizedAccessException)
            {
                Status = "Login failed! Please provide some valid credentials.";
                ProgressText = "Login failed!";
                NotifyPropertyChanged("Status");
            }
            catch (Exception ex)
            {
                Status = string.Format("ERROR: {0}", ex.Message);
                ProgressText = "Login failed!";
                NotifyPropertyChanged("Status");
            }
        }

        private void workerProgressBar_DoWork(object sender, DoWorkEventArgs e)
        {
            ProgressVisibility = Visibility.Visible;

            for (int i = 10; i < 100; i = i + 10)
            {
                if (ProgressText != "Login failed!")
                {
                    if (i == 60)
                    {
                        ProgressText = "Just a second...";
                    }
                    ProgressBarValue = i;
                    Thread.Sleep(230);
                }
                else
                {
                    break;
                }
            }
        }

        private void Login(object parameter)
        {
            BackgroundWorker workerLogin = new BackgroundWorker();
            workerLogin.DoWork += workerLogin_DoWork;
            workerLogin.RunWorkerAsync(parameter);

            BackgroundWorker workerProgressBar = new BackgroundWorker();
            workerProgressBar.DoWork += workerProgressBar_DoWork;
            workerProgressBar.RunWorkerAsync();
        }

        private bool CanLogin(object parameter)
        {
            return !IsAuthenticated;
        }

        public bool IsAuthenticated
        {
            get { return Thread.CurrentPrincipal.Identity.IsAuthenticated; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}