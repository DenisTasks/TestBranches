using System.Windows;
using System.Windows.Controls;

namespace TestWpf.Controls
{
    public class CalendarLedgerItem : Control
    {
        static CalendarLedgerItem()
        {
           DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarLedgerItem), new FrameworkPropertyMetadata(typeof(CalendarLedgerItem)));
        }

        #region TimeslotA
        public static readonly DependencyProperty TimeslotAProperty = DependencyProperty.Register("TimeslotA", typeof(string), typeof(CalendarLedgerItem), new FrameworkPropertyMetadata(string.Empty));

        public string TimeslotA
        {
            get => (string)GetValue(TimeslotAProperty);
            set => SetValue(TimeslotAProperty, value);
        }
        #endregion

        #region TimeslotB
        public static readonly DependencyProperty TimeslotBProperty = DependencyProperty.Register("TimeslotB", typeof(string), typeof(CalendarLedgerItem), new FrameworkPropertyMetadata(string.Empty));

        public string TimeslotB
        {
            get => (string)GetValue(TimeslotBProperty);
            set => SetValue(TimeslotBProperty, value);
        }
        #endregion
    }
}