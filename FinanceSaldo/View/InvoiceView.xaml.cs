using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using FinanceSaldo.View.Extensions;

namespace FinanceSaldo.View
{
    /// <summary>
    /// Description for EmployeeInfoView.
    /// </summary>
    public partial class InvoiceView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the EmployeeInfoView class.
        /// </summary>
        public InvoiceView()
        {
            InitializeComponent();
        }

        private void DgInvoice_LayoutUpdated(object sender, EventArgs e)
        {
            //Thickness t = TbTotal.Margin;
            //t.Left = DgInvoice.Columns[0].ActualWidth + 7;
            //TbTotal.Margin = t;
            TbTotal.Width = DgInvoice.Columns[0].ActualWidth + DgInvoice.Columns[1].ActualWidth + 7;
            TbTotalDebit.Width = DgInvoice.Columns[2].ActualWidth;
            TbTotalCredit.Width = DgInvoice.Columns[3].ActualWidth;

            TbTotalCash.Width = TbTotal.Width;
            TbTotalCashDebit.Width = TbTotalDebit.Width;
            TbTotalCashCredit.Width = TbTotalCredit.Width;
        }
    }
}