using System.Windows;

namespace TestWpf.Calendar
{
    /// <summary>
    /// Interaction logic for CalendarFrame.xaml
    /// </summary>
    public partial class CalendarFrame : Window
    {
        public CalendarFrame()
        {
            InitializeComponent();
            int start = 0;
            CalendarPage calendar = new CalendarPage();
            mainCalendarFrame.NavigationService.LoadCompleted += calendar.NavigationService_LoadCompleted;
            mainCalendarFrame.NavigationService.Navigate(calendar, start);
        }
    }
}
