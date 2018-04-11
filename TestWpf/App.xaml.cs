using System;
using System.Windows;
using GalaSoft.MvvmLight.Threading;
using ViewModel.Helpers;
using ViewModel.ViewModels.Authenication;

namespace TestWpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            CustomPrincipal customPrincipal = new CustomPrincipal();
            AppDomain.CurrentDomain.SetThreadPrincipal(customPrincipal);

            base.OnStartup(e);
            
            //AuthenticationViewModel viewModel = new AuthenticationViewModel();
            //IView loginWindow = new LoginWindow(viewModel);
            //loginWindow.Show();
        }

        static App()
        {
            DispatcherHelper.Initialize();
            AutoMapperConfig.RegisterMappings();
        }
    }
}