using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TestWpf.Controls;

namespace TestWpf.Calendar
{
    /// <summary>
    /// Interaction logic for CalendarPage.xaml
    /// </summary>
    public partial class CalendarPage : Page
    {
        private int _start;
        public CalendarPage()
        {
            InitializeComponent();
        }

        public void NavigationService_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _start = Convert.ToInt32(e.ExtraData);
            CalendarListView.View.SetValue(CalendarView.StartDayProperty, _start);
            CalendarListView.View.SetValue(CalendarView.FinishDayProperty, _start + 7);
            this.NavigationService.LoadCompleted -= NavigationService_LoadCompleted;

            SyncButton.CommandParameter = _start;
        }

        private void ButtonBase_OnNextWeekClick(object sender, RoutedEventArgs e)
        {
            CalendarPage calendar = new CalendarPage();
            this.NavigationService.LoadCompleted += calendar.NavigationService_LoadCompleted;
            this.NavigationService.Navigate(calendar, _start + 7);
        }

        private void ButtonBase_OnPreviousWeekClick(object sender, RoutedEventArgs e)
        {
            CalendarPage calendar = new CalendarPage();
            this.NavigationService.LoadCompleted += calendar.NavigationService_LoadCompleted;
            this.NavigationService.Navigate(calendar, _start - 7);
        }
    }
}