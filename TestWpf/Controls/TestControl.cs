using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TestWpf.Controls
{
    public class TestControl : Button
    {
        public TestControl() : base()
        {
            SetResourceReference(StyleProperty, typeof(Button));
        }

        public static readonly DependencyProperty CurrentTimeProperty = DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(TestControl), 
                new FrameworkPropertyMetadata(DateTime.Now, OnCurrentTimePropertyChanged));

        public DateTime CurrentTime
        {
            get => (DateTime)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }

        private static void OnCurrentTimePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            //MessageBox.Show("123213");
        }
    }
}