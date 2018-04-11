using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using TestWpf.Controls;

namespace TestWpf.Primitives
{
    // area "Content"
    public class CalendarViewContentPresenter : Panel
    {
        private UIElementCollection _visualChildren;
        private bool _visualChildrenGenerated;
        private List<UIElement> _listViewItemVisuals;
        private bool _listViewItemVisualsGenerated;

        protected CalendarView CalendarView => ListView.View as CalendarView;

        protected ListView ListView => TemplatedParent as ListView;

        internal List<UIElement> ListViewItemVisuals => _listViewItemVisuals;

        // set x/y for children (days). Input - size of all window (area "Content")
        protected override Size ArrangeOverride(Size finalSize)
        {
            int columnCount = CalendarView.Periods.Count;
            Size columnSize = new Size(finalSize.Width / columnCount, finalSize.Height);
            double elementX = 0;
            foreach (UIElement element in _visualChildren)
            {
                element.Arrange(new Rect(new Point(elementX, 0), columnSize));
                elementX = elementX + columnSize.Width;
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            GenerateVisualChildren();
            GenerateListViewItemVisuals();

            return constraint;
        }

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

        protected void GenerateVisualChildren()
        {
            if (_visualChildrenGenerated)
                return;

            if (_visualChildren == null)
                _visualChildren = CreateUIElementCollection(this);
            else
                _visualChildren.Clear();

            foreach (var period in CalendarView.Periods)
            {

                _visualChildren.Add(new CalendarViewPeriodPresenter { Period = period, CalendarView = CalendarView, ListView = ListView });
            }

            _visualChildrenGenerated = true;
        }

        protected void GenerateListViewItemVisuals()
        {
            if (_listViewItemVisualsGenerated)
                return;

            IItemContainerGenerator generator = ((IItemContainerGenerator)ListView.ItemContainerGenerator).GetItemContainerGeneratorForPanel(this);
            generator.RemoveAll();

            if (_listViewItemVisuals == null)
                _listViewItemVisuals = new List<UIElement>();
            else
                _listViewItemVisuals.Clear();

            using (generator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward))
            {
                UIElement element;
                while ((element = generator.GenerateNext() as UIElement) != null)
                {
                    _listViewItemVisuals.Add(element);
                    generator.PrepareItemContainer(element);
                }

                _listViewItemVisualsGenerated = true;
            }
        }
    }
}