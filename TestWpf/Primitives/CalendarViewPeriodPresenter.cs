using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using TestWpf.Controls;

namespace TestWpf.Primitives
{
    public class CalendarViewPeriodPresenter : Panel
    {
        private bool _visualChildrenGenerated;
        private UIElementCollection _visualChildren;

        static CalendarViewPeriodPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CalendarViewPeriodPresenter), new FrameworkPropertyMetadata(typeof(CalendarViewPeriodPresenter)));
        }

        public CalendarViewPeriod Period { get; set; }

        public ListView ListView { get; set; }

        public CalendarView CalendarView { get; set; }

        private CalendarViewContentPresenter ContentPresenter => (CalendarViewContentPresenter)Parent;

        protected override int VisualChildrenCount
        {
            get
            {
                if (_visualChildren == null)
                    return base.VisualChildrenCount;

                return _visualChildren.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if ((index < 0) || (index >= VisualChildrenCount))
                throw new ArgumentOutOfRangeException("index", index, "Index out of range");

            if (_visualChildren == null)
                return base.GetVisualChild(index);

            return _visualChildren[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in _visualChildren)
                element.Arrange(new Rect(new Point(0, 0), finalSize));

            return finalSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            GenerateVisualChildren();

            return constraint;
        }

        protected void GenerateVisualChildren()
        {
            if (_visualChildrenGenerated)
                return;

            if (_visualChildren == null)
                _visualChildren = CreateUIElementCollection(null);
            else
                _visualChildren.Clear();

            RangePanel panel = new RangePanel();

            // PERIOD
            panel.SetBinding(RangePanel.MinimumHeightProperty, new Binding("BeginDate.Ticks") { Source = Period });
            panel.SetBinding(RangePanel.MaximumHeightProperty, new Binding("EndDate.Ticks") { Source = Period });



            foreach (ListViewItem item in ContentPresenter.ListViewItemVisuals)
            {
                if (CalendarView.PeriodContainsItem(item, Period))
                {
                    item.SetValue(RangePanel.StartProperty, Convert.ToDouble(((DateTime)item.GetValue(CalendarView.BeginDateProperty)).Ticks));
                    item.SetValue(RangePanel.FinishProperty, Convert.ToDouble(((DateTime)item.GetValue(CalendarView.EndDateProperty)).Ticks));
                    item.SetValue(RangePanel.StartDayOfYearProperty, Convert.ToDateTime(item.GetValue(CalendarView.BeginDateProperty)).DayOfYear);
                    item.SetValue(RangePanel.FinishDayOfYearProperty, Convert.ToDateTime(item.GetValue(CalendarView.EndDateProperty)).DayOfYear);

                    panel.Children.Add(item);
                }
            }

            Border border = new Border
            {
                BorderBrush = Brushes.Orange,
                BorderThickness = new Thickness(2.0),
                CornerRadius = new CornerRadius(10, 10, 10, 10),
                Child = panel
            };
            _visualChildren.Add(border);

            _visualChildrenGenerated = true;
        }
    }
}