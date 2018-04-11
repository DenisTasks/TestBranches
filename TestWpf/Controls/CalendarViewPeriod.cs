using System;
using System.Windows;

namespace TestWpf.Controls
{
    // day
    public class CalendarViewPeriod : DependencyObject
    {
        public static readonly DependencyProperty BeginDateProperty = DependencyProperty.Register("BeginDate", typeof(DateTime), typeof(CalendarViewPeriod));
        public static readonly DependencyProperty EndDateProperty = DependencyProperty.Register("EndDate", typeof(DateTime), typeof(CalendarViewPeriod));

        public DateTime BeginDate
        {
            get => (DateTime)GetValue(BeginDateProperty);
            set => SetValue(BeginDateProperty, value);
        }

        public DateTime EndDate
        {
            get => (DateTime)GetValue(EndDateProperty);
            set => SetValue(EndDateProperty, value);
        }
    }
}
