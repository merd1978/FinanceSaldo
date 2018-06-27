 using System.Windows;
using System.Windows.Controls;

namespace FinanceSaldo.View
{
    /// <summary>
    /// Description for EmployeeInfoView.
    /// </summary>
    public partial class CompanyView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the EmployeeInfoView class.
        /// </summary>
        public CompanyView()
        {
            InitializeComponent();
        }

        private void TbMain_Loaded(object sender, RoutedEventArgs e)
        {
            if (!(sender is ToolBar toolBar)) return;
            foreach (FrameworkElement a in TbMain.Items)
            {
                ToolBar.SetOverflowMode(a, OverflowMode.Never);
            }
            if (toolBar.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }
        }
    }
}