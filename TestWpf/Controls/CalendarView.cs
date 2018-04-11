using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TestWpf.Controls
{
    public class CalendarView : ViewBase
    {
        private DateTime _dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);

        public static readonly DependencyProperty StartDayProperty = DependencyProperty.Register("StartDay", typeof(int), typeof(CalendarView));
        public static readonly DependencyProperty FinishDayProperty = DependencyProperty.Register("FinishDay", typeof(int), typeof(CalendarView));
        public static DependencyProperty BeginDateProperty = DependencyProperty.RegisterAttached("BeginDate", typeof(DateTime), typeof(ListViewItem));
        public static DependencyProperty EndDateProperty = DependencyProperty.RegisterAttached("EndDate", typeof(DateTime), typeof(ListViewItem));
        
        private ObservableCollection<CalendarViewPeriod> _periods;
        
        public BindingBase ItemBeginDateBinding { get; set; }
        public BindingBase ItemEndDateBinding { get; set; }

        public ObservableCollection<CalendarViewPeriod> Periods
        {
            get
            {
                if (_periods == null)
                    _periods = GetWeek();
                return _periods;
            }
        }

        public ObservableCollection<CalendarViewPeriod> GetWeek()
        {
            var week = new ObservableCollection<CalendarViewPeriod>();
            for (int i = StartDay; i < FinishDay; i++)
            {
                week.Add(new CalendarViewPeriod { BeginDate = _dt.AddDays(i), EndDate = _dt.AddDays(i).AddHours(23).AddMinutes(59).AddSeconds(59) });
            }
            return week;
        }

        public int StartDay
        {
            get => (int)GetValue(StartDayProperty);
            set => SetValue(StartDayProperty, value);
        }
        public int FinishDay
        {
            get => (int)GetValue(FinishDayProperty);
            set => SetValue(FinishDayProperty, value);
        }

        public static DateTime GetBegin(DependencyObject item)
        {
            return (DateTime)item.GetValue(BeginDateProperty);
        }
        public static DateTime GetEnd(DependencyObject item)
        {
            return (DateTime)item.GetValue(EndDateProperty);
        }
        public static void SetBegin(DependencyObject item, DateTime value)
        {
            item.SetValue(BeginDateProperty, value);
        }
        public static void SetEnd(DependencyObject item, DateTime value)
        {
            item.SetValue(EndDateProperty, value);
        }

        protected override void PrepareItem(ListViewItem item)
        {
            item.SetBinding(BeginDateProperty, ItemBeginDateBinding);
            item.SetBinding(EndDateProperty, ItemEndDateBinding);
        }

        public bool PeriodContainsItem(ListViewItem item, CalendarViewPeriod period)
        {
            DateTime itemBegin = (DateTime)item.GetValue(BeginDateProperty);
            DateTime itemEnd = (DateTime)item.GetValue(EndDateProperty);

            return (((itemBegin <= period.BeginDate) && (itemEnd >= period.BeginDate)) || ((itemBegin <= period.EndDate) && (itemEnd >= period.BeginDate)));
        }

        protected override object DefaultStyleKey => new ComponentResourceKey(GetType(), "DefaultStyleKey");
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            System.Globalization.CultureInfo ci = System.Threading.Thread.CurrentThread.CurrentCulture;
            DayOfWeek fdow = ci.DateTimeFormat.FirstDayOfWeek;
            return DateTime.Today.AddDays(-(DateTime.Today.DayOfWeek - fdow));
        }
    }
}