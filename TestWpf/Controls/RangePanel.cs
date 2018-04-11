using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TestWpf.Controls
{
    public class RangePanel : Panel
    {
        public static DependencyProperty MinimumHeightProperty = DependencyProperty.Register("MinimumHeight", typeof(double), typeof(RangePanel), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));
        public static DependencyProperty MaximumHeightProperty = DependencyProperty.Register("MaximumHeight", typeof(double), typeof(RangePanel), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public static DependencyProperty StartProperty = DependencyProperty.RegisterAttached("Start", typeof(double), typeof(UIElement), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsArrange));
        public static DependencyProperty FinishProperty = DependencyProperty.RegisterAttached("Finish", typeof(double), typeof(UIElement), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsArrange));
        public static DependencyProperty StartDayOfYearProperty = DependencyProperty.RegisterAttached("StartDayOfYear", typeof(int), typeof(UIElement), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange));
        public static DependencyProperty FinishDayOfYearProperty = DependencyProperty.RegisterAttached("FinishDayOfYear", typeof(int), typeof(UIElement), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsArrange));

        public static void SetStartDayOfYear(UIElement element, int value)
        {
            element.SetValue(StartDayOfYearProperty, value);
        }
        public static int GetStartDayOfYear(UIElement element)
        {
            return (int) element.GetValue(StartDayOfYearProperty);
        }
        public static void SetFinishDayOfYear(UIElement element, int value)
        {
            element.SetValue(FinishDayOfYearProperty, value);
        }
        public static int GetFinishDayOfYear(UIElement element)
        {
            return (int)element.GetValue(FinishDayOfYearProperty);
        }
        public static void SetStart(UIElement element, double value)
        {
            element.SetValue(StartProperty, value);
        }
        public static double GetStart(UIElement element)
        {
            return (double)element.GetValue(StartProperty);
        }
        public static void SetFinish(UIElement element, double value)
        {
            element.SetValue(FinishProperty, value);
        }
        public static double GetFinish(UIElement element)
        {
            return (double)element.GetValue(FinishProperty);
        }

        public double MaximumHeight
        {
            get => (double)GetValue(MaximumHeightProperty);
            set => SetValue(MaximumHeightProperty, value);
        }
        public double MinimumHeight
        {
            get => (double)GetValue(MinimumHeightProperty);
            set => SetValue(MinimumHeightProperty, value);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double containerRangeHeigth = MaximumHeight - MinimumHeight;
            List<UIElement> uiAll = new List<UIElement>();
            List<UIElement> uiOverlapping = new List<UIElement>();

            foreach (UIElement item in Children)
            {
                uiAll.Add(item);
            }

            for (int i = 0; i < uiAll.Count; i++)
            {
                double begin = (double)uiAll.ElementAt(i).GetValue(StartProperty);
                double end = (double)uiAll.ElementAt(i).GetValue(FinishProperty);
                int dayOfYear = (int)uiAll.ElementAt(i).GetValue(StartDayOfYearProperty);

                var forOverlap = uiAll.Where(s => (double)s.GetValue(FinishProperty) > begin 
                && (double)s.GetValue(StartProperty) < end
                && (int)s.GetValue(StartDayOfYearProperty) == dayOfYear).ToList();

                foreach (var item in forOverlap)
                {
                    if (!uiOverlapping.Contains(item) && forOverlap.Count > 1)
                    {
                        uiOverlapping.Add(item);
                    }
                }
            }

            Size widthOverlap = new Size {Width = finalSize.Width / uiOverlapping.Count};
            Point locationX = new Point {X = 0};

            foreach (UIElement element in Children)
            {
                if (uiOverlapping.Contains(element))
                {
                    double begin = (double)element.GetValue(StartProperty);
                    double end = (double)element.GetValue(FinishProperty);
                    double elementRange = end - begin;

                    Size size = new Size();
                    size.Width = widthOverlap.Width; // property for overlapped appointment
                    size.Height = elementRange / containerRangeHeigth * finalSize.Height;

                    Point location = new Point();
                    location.X = locationX.X; // property for overlapped appointment
                    location.Y = (begin - MinimumHeight) / containerRangeHeigth * finalSize.Height;

                    element.Arrange(new Rect(location, size));

                    widthOverlap.Width = finalSize.Width / uiOverlapping.Count;
                    locationX.X = locationX.X + finalSize.Width / uiOverlapping.Count;
                }
                else
                {
                    double begin = (double)element.GetValue(StartProperty);
                    double end = (double)element.GetValue(FinishProperty);
                    double elementRange = end - begin;

                    Size size = new Size
                    {
                        Width = finalSize.Width,
                        Height = elementRange / containerRangeHeigth * finalSize.Height
                    };

                    Point location = new Point
                    {
                        X = 0,
                        Y = (begin - MinimumHeight) / containerRangeHeigth * finalSize.Height
                    };

                    element.Arrange(new Rect(location, size));
                }
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement element in Children)
            {
                element.Measure(availableSize);
            }
            return availableSize;
        }
    }
}