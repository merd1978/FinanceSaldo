using System.Windows.Controls;
using System.Windows.Interactivity;

namespace FinanceSaldo.View.Extensions
{
    public class ScrollOnNewItemBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = sender as DataGrid;
            if (grid?.SelectedItem != null)
            {
                grid.Dispatcher.InvokeAsync(() =>
                {
                    grid.UpdateLayout();
                    grid.ScrollIntoView(grid.SelectedItem, null);
                });
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }
    }
}
