using System.Collections.ObjectModel;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FinanceSaldo.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FinanceSaldo.ViewModel
{
    public class InvoiceViewModel : TabViewModelBase
    {
        private readonly IDataService _dataService;

        private CompanyList _сompanyList;
        public CompanyList CompanyList
        {
            get => _сompanyList;
            set => Set(ref _сompanyList, value);
        }

        ObservableCollection<Invoice> _invoice;
        public ObservableCollection<Invoice> Invoice
        {
            get => _invoice;
            set => Set(ref _invoice, value);
        }

        private Invoice _selectedInvoice;
        public Invoice SelectedInvoice
        {
            get => _selectedInvoice;
            set => Set(ref _selectedInvoice, value);
        }

        public string SearchText { get; set; }

        public RelayCommand SaveCommand { get; set; }
        private void ExecuteSaveCommand()
        {
            CompanyList.Company.Invoice = Invoice;
            _dataService.UpdateCompany(CompanyList.Company);
            Messenger.Default.Send(new NotificationMessage("UpdateCompany"));
        }

        public RelayCommand DeleteCommand { get; set; }
        private void ExecuteDeleteCommand()
        {
            _dataService.RemoveInvoice(SelectedInvoice);
            Invoice.Remove(SelectedInvoice);
            Messenger.Default.Send(new NotificationMessage("UpdateCompany"));
        }

        public RelayCommand CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand()
        {
            Messenger.Default.Send(new NotificationMessage("CloseCurrentTab"));
        }

        public RelayCommand<DataGrid> PrintCommand { get; set; }
        private void ExecutePrintCommand(DataGrid dataGrid)
        {
            PrintDialog printdlg = new PrintDialog();
            if ((bool) printdlg.ShowDialog().GetValueOrDefault())
            {
                Size pageSize = new Size(printdlg.PrintableAreaWidth, printdlg.PrintableAreaHeight);
                dataGrid.Measure(pageSize);
                dataGrid.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
                printdlg.PrintVisual(dataGrid, "Печать отчета");
            }
        }

        public RelayCommand SearchCommand { get; set; }
        private void ExecuteSearchCommand()
        {
            foreach (Invoice inv in Invoice)
            {
                if (inv.Name != null && inv.Name.ToLower().Contains(SearchText))
                {
                    SelectedInvoice = inv;
                }
            }
        }


        public InvoiceViewModel(IDataService dataService, CompanyList companyList) : base(companyList.Company.Name)
        {
            _dataService = dataService;
            CompanyList = companyList;
            Invoice = new ObservableCollection<Invoice>(companyList.Company.Invoice);

            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);
            PrintCommand = new RelayCommand<DataGrid>(ExecutePrintCommand);
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
        }
    }
}