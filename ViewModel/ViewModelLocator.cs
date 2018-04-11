using BLL.BLLService;
using BLL.Interfaces;
using BLL.Services;
using GalaSoft.MvvmLight;
using Model;
using Model.Interfaces;
using Model.ModelService;
using Ninject;
using Ninject.Modules;
using ViewModel.ViewModels.Administration;
using ViewModel.ViewModels.Administration.Groups;
using ViewModel.ViewModels.Administration.Users;
using ViewModel.ViewModels.Appointments;
using ViewModel.ViewModels.Authenication;
using ViewModel.ViewModels.Calendar;
using ViewModel.ViewModels.CommonViewModels.Groups;

namespace ViewModel
{
    public class DesignTimeModule : NinjectModule
    {
        public override void Load()
        {
        }
    }

    public class RunTimeModule : NinjectModule
    {
        public override void Load()
        {
            Bind<WPFOutlookContext>().ToSelf().InSingletonScope().WithConstructorArgument("connectionString", "WPFOutlookContext");
            Bind(typeof(IGenericRepository<>)).To(typeof(GenericRepository<>)).InSingletonScope();
            Bind<IBLLServiceMain>().To<BLLServiceMain>().InSingletonScope();
            Bind<IAuthenticationService>().To<AuthenticationService>().InSingletonScope();
            Bind<IAdministrationService>().To<AdministrationService>().InSingletonScope();
            Bind<ILogService>().To<LogService>().InSingletonScope();
            Bind<INotifyService>().To<NotifyService>().InSingletonScope();
        }
    }

    public class ViewModelLocator
    {
        private readonly IKernel _kernel;

        public MainWindowViewModel MainWindow => _kernel.Get<MainWindowViewModel>();
        public AddAppWindowViewModel AddAppWindow => _kernel.Get<AddAppWindowViewModel>();
        public AboutAppointmentWindowViewModel AboutAppWindow => _kernel.Get<AboutAppointmentWindowViewModel>();
        public AllAppByLocationWindowViewModel AllAppByLocWindow => _kernel.Get<AllAppByLocationWindowViewModel>();
        public ToastListViewModel ToastWindow => _kernel.Get<ToastListViewModel>();
        public SyncWindowViewModel SyncWindow => _kernel.Get<SyncWindowViewModel>();
        public AuthenticationViewModel LoginPage => _kernel.Get<AuthenticationViewModel>();
        public AdministrationViewModel AdminPage => _kernel.Get<AdministrationViewModel>();
        public EditGroupViewModel EditGroupWindow => _kernel.Get<EditGroupViewModel>();
        public ShowAllGroupsViewModel AllGroupsPageTest => _kernel.Get<ShowAllGroupsViewModel>();
        public AddGroupViewModel AddGroupWindow => _kernel.Get<AddGroupViewModel>();
        public AddUserViewModel AddUserWindow => _kernel.Get<AddUserViewModel>();
        public EditUserViewModel EditUserWindow => _kernel.Get<EditUserViewModel>();
        public ShowAllUsersViewModel AllUsersPage => _kernel.Get<ShowAllUsersViewModel>();
        public ShowAllLogsViewModel AllLogsPage => _kernel.Get<ShowAllLogsViewModel>();
        public CalendarWindowViewModel CalendarPage => _kernel.Get<CalendarWindowViewModel>();

        public ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                _kernel = new StandardKernel(new DesignTimeModule());
            }
            else
            {
                _kernel = new StandardKernel(new RunTimeModule());
            }
        }
    }
}