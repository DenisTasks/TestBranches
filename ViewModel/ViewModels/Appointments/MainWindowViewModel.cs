using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using AutoMapper;
using BLL.EntitesDTO;
using BLL.Interfaces;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OutlookLocalization;
using ViewModel.Helpers;
using ViewModel.Models;
using ViewModel.Notify;
using ViewModel.ViewModels.Authenication;

namespace ViewModel.ViewModels.Appointments
{
    [ServiceContract]
    public interface IMyObject
    {
        [OperationContract]
        void GetCommand();
    }

    public class MyObject : IMyObject
    {
        public void GetCommand()
        {
            MessageBox.Show(DateTime.Now.ToString("D"));
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IBLLServiceMain _service;
        private readonly ILogService _logService;
        private readonly INotifyService _notifyService;
        private readonly int _id;
        private readonly DispatcherTimer _timer;
        private CultureInfo _currentCulture;
        private ServiceHost _host;

        private string _dateTimeNow;
        private ObservableCollection<AppointmentModel> _appointments;
        private ObservableCollection<FileInfo> _files;
        private ObservableCollection<string> _languages;
        private FileInfo _selectedTheme;
        private string _selectedLanguage;

        public string CurrentDateTime
        {
            get => _dateTimeNow;
            set
            {
                _dateTimeNow = value;
                base.RaisePropertyChanged();
            }
        }
        public ObservableCollection<AppointmentModel> Appointments
        {
            get => _appointments;
            set
            {
                if (value != _appointments)
                {
                    _appointments = value;
                    base.RaisePropertyChanged();
                }
            }
        }
        public ObservableCollection<FileInfo> Files
        {
            get => _files;
            set
            {
                _files = value;
                base.RaisePropertyChanged();
            }
        }
        public ObservableCollection<string> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                base.RaisePropertyChanged();
            }
        }

        public FileInfo SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                _selectedTheme = value;
                base.RaisePropertyChanged();
                ChangeTheme(_selectedTheme);
            }
        }
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                base.RaisePropertyChanged();
                ChangeLanguage(_selectedLanguage);
            }
        }


        #region Commands
        public RelayCommand<AppointmentModel> PrintAppointmentCommand { get; }
        public RelayCommand<AppointmentModel> AllAppByLocationCommand { get; }
        public RelayCommand AddAppWindowCommand { get; }
        public RelayCommand<AppointmentModel> RemoveAppCommand { get; }
        public RelayCommand<object> SortCommand { get; }
        public RelayCommand GroupBySubjectCommand { get; }
        public RelayCommand<AppointmentModel> FilterBySubjectCommand { get; }
        public RelayCommand<object> PrintTable { get; }
        public RelayCommand CalendarFrameCommand { get; }
        public RelayCommand CreateGroupCommand { get; }
        #endregion

        public MainWindowViewModel(IBLLServiceMain service, ILogService logService, INotifyService notifyService)
        {
            CustomPrincipal cp = (CustomPrincipal)Thread.CurrentPrincipal;
            if (cp != null) _id = cp.Identity.UserId;
            _service = service;
            _logService = logService;
            _notifyService = notifyService;

            //_host = new ServiceHost(typeof(MyObject), new Uri("http://localhost:1050/TestService"));
            //_host.AddServiceEndpoint(typeof(IMyObject), new BasicHttpBinding(), "");
            //_host.Open();

            Scheduler.RemoveFromDataBase += RemoveNotifyFromDatabase;
            LoadData();

            #region Commands
            AddAppWindowCommand = new RelayCommand(AddAppointment);
            PrintAppointmentCommand = new RelayCommand<AppointmentModel>(PrintAppointment);
            AllAppByLocationCommand = new RelayCommand<AppointmentModel>(GetAllAppsByRoom);
            RemoveAppCommand = new RelayCommand<AppointmentModel>(RemoveAppointment);
            SortCommand = new RelayCommand<object>(SortBy);
            GroupBySubjectCommand = new RelayCommand(GroupBySubject);
            FilterBySubjectCommand = new RelayCommand<AppointmentModel>(FilterBySubject);
            PrintTable = new RelayCommand<object>(PrintListView);
            CalendarFrameCommand = new RelayCommand(CalendarFrame);
            CreateGroupCommand = new RelayCommand(CreateGroup);
            #endregion

            #region Themes
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Resources");
            var localthemes = new DirectoryInfo(path).GetFiles();
            Files = new ObservableCollection<FileInfo>();
            foreach (var item in localthemes)
            {
                Files.Add(item);
            }
            SelectedTheme = Files[1];
            #endregion

            #region Timer
            _currentCulture = CultureInfo.CurrentCulture;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
            _timer.Tick += TimerOnTick;

            BackgroundWorker workerTimer = new BackgroundWorker();
            workerTimer.WorkerSupportsCancellation = true;
            workerTimer.DoWork += workerTimer_DoWork;
            workerTimer.RunWorkerAsync();
            #endregion

            Languages = new ObservableCollection<string> {"English", "Russian"};
            SelectedLanguage = Languages[0];

            Messenger.Default.Register<NotificationMessage>(this, message =>
            {
                if (message.Notification == "Refresh")
                {
                    RefreshingAppointments();
                }
                if (message.Notification == "LogOut")
                {
                    Messenger.Default.Unregister<NotificationMessage>(this);

                    _timer.Tick -= TimerOnTick;
                    _timer?.Stop();

                    // ReSharper disable once DelegateSubtraction
                    Scheduler.RemoveFromDataBase -= RemoveNotifyFromDatabase;
                    Scheduler.Shutdown();

                    _host?.Close();
                }
            });
        }

        private void workerTimer_DoWork(object sender, DoWorkEventArgs e)
        {
            _timer?.Start();
        }
        private void TimerOnTick(object sender, EventArgs e)
        {
            CurrentDateTime = DateTime.Now.ToString("F", _currentCulture);
        }
        public void RemoveNotifyFromDatabase(object sender, NotifyEventArgs args)
        {
            _notifyService.RemoveFromNotification(args.NotifyId, _id);
        }
        private void LoadData()
        {
            try
            {
                Appointments = new ObservableCollection<AppointmentModel>(Mapper.Map<IEnumerable<AppointmentDTO>, IEnumerable<AppointmentModel>>(_service.GetAppointmentsByUserId(_id)));

                Scheduler.ScheduleNotify(_notifyService.GetNotificationsByUserId(_id));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        private void CreateGroup()
        {
            Messenger.Default.Send(new NotificationMessage("CreateGroup"));
        }
        private void PrintListView(object parameter)
        {
            PrintHelper.PrintViewList(parameter as ListView);
        }
        private void ChangeTheme(FileInfo selectedTheme)
        {
            Application.Current.Resources.Clear();
            Application.Current.Resources.Source = new Uri(selectedTheme.FullName, UriKind.Absolute);
        }
        private void ChangeLanguage(string selectedLanguage)
        {
            switch (selectedLanguage)
            {
                case "English":
                    _currentCulture = new CultureInfo("en-US");
                    break;
                case "Russian":
                    _currentCulture = new CultureInfo("ru-RU");
                    break;
            }

            LocalizationManager.UICulture = new CultureInfo(_currentCulture.Name);
        }
        private void AddAppointment()
        {
            Messenger.Default.Send(new OpenWindowMessage { Type = WindowType.AddAppWindow });
        }
        private void CalendarFrame()
        {
            Messenger.Default.Send(new OpenWindowMessage { Type = WindowType.CalendarFrame });
        }
        private void PrintAppointment(AppointmentModel appointment)
        {
            //
            //string messageString = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            //char[] message = messageString.ToCharArray();
            //int size = message.Length;
            //MemoryMappedFile sharedMemory = MemoryMappedFile.CreateOrOpen("MemoryFile", size * 2 + 4);
            //using (MemoryMappedViewAccessor writer = sharedMemory.CreateViewAccessor(0, size * 2 + 4))
            //{
            //    writer.Write(0, size);
            //    writer.WriteArray(4, message, 0, message.Length);
            //}

            if (appointment != null)
            {
                Messenger.Default.Send(new OpenWindowMessage { Type = WindowType.AddAboutAppointmentWindow, Appointment = appointment, Argument = "Load this appointment" });
            }
        }
        private void RefreshingAppointments()
        {
            Appointments = new ObservableCollection<AppointmentModel>(Mapper.Map<IEnumerable<AppointmentDTO>, IEnumerable<AppointmentModel>>(_service.GetAppointmentsByUserId(_id)));

            Messenger.Default.Send(new OpenWindowMessage { Type = WindowType.Toast, Argument = "You added a new\r\nappointment! Check\r\nyour calendar, please!", SecondsToShow = 5 });

            var lastNotify = _notifyService.GetNotificationsByUserId(_id).LastOrDefault();
            if (lastNotify != null)
            {
                Scheduler.ScheduleNotify(new ObservableCollection<NotificationDTO> { lastNotify });
            }
        }
        private void GetAllAppsByRoom(AppointmentModel appointment)
        {
            //char[] message;
            //int size;
            //MemoryMappedFile sharedMemory = MemoryMappedFile.OpenExisting("MemoryFile");
            //using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(0, 4, MemoryMappedFileAccess.Read))
            //{
            //    size = reader.ReadInt32(0);
            //}
            //using (MemoryMappedViewAccessor reader = sharedMemory.CreateViewAccessor(4, size * 2, MemoryMappedFileAccess.Read))
            //{
            //    message = new char[size];
            //    reader.ReadArray(0, message, 0, size);
            //}
            //string s = new string(message);
            //MessageBox.Show(s);

            if (appointment != null)
            {
                try
                {
                    Messenger.Default.Send(new OpenWindowMessage()
                    { Type = WindowType.AddAllAppByLocationWindow, Argument = appointment.LocationId.ToString() });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            else
            {
                //Uri tcpUri = new Uri("http://localhost:1050/TestService");
                //EndpointAddress address = new EndpointAddress(tcpUri);
                //BasicHttpBinding binding = new BasicHttpBinding();
                //ChannelFactory<IMyObject> factory = new ChannelFactory<IMyObject>(binding, address);
                //IMyObject service = factory.CreateChannel();

                //service.GetCommand();
            }
        }
        private void GroupBySubject()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(Appointments);
            view.GroupDescriptions.Clear();
            view.GroupDescriptions.Add(new PropertyGroupDescription("Subject"));
        }
        private void SortBy(object parameter)
        {
            string column = (string)parameter;
            if (column != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(Appointments);
                if (view.SortDescriptions.Count > 0
                    && view.SortDescriptions[0].PropertyName == column
                    && view.SortDescriptions[0].Direction == ListSortDirection.Ascending)
                {
                    view.GroupDescriptions.Clear();
                    view.Filter = null;
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(column, ListSortDirection.Descending));
                }
                else
                {
                    view.GroupDescriptions.Clear();
                    view.Filter = null;
                    view.SortDescriptions.Clear();
                    view.SortDescriptions.Add(new SortDescription(column, ListSortDirection.Ascending));
                }
            }
        }
        private void FilterBySubject(AppointmentModel appointment)
        {
            if (appointment != null)
            {
                ICollectionView view = CollectionViewSource.GetDefaultView(Appointments);
                view.GroupDescriptions.Clear();
                view.Filter = s => ((s as AppointmentModel)?.Subject) == appointment.Subject;
            }
        }
        private void RemoveAppointment(AppointmentModel appointment)
        {
            if (appointment != null)
            {
                try
                {
                    CustomPrincipal customPrincipal = Thread.CurrentPrincipal as CustomPrincipal;
                    if (customPrincipal != null)
                        _logService.LogAppointment(Mapper.Map<AppointmentModel, AppointmentDTO>(appointment),
                            customPrincipal.Identity.UserId, false);

                    _service.RemoveFromAppointment(appointment.AppointmentId, _id);
                    _notifyService.RemoveNotification(appointment.AppointmentId, _id);

                    Appointments.Remove(appointment);

                    base.RaisePropertyChanged();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }
    }
}