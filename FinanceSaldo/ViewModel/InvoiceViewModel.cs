using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using FinanceSaldo.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FinanceSaldo.ViewModel
{
    public class InvoiceViewModel : TabViewModelBase
    {
        private readonly IDataService _dataService;

        private Company _сompany;
        public Company Company
        {
            get => _сompany;
            set => Set(ref _сompany, value);
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
            set
            {
                Set(ref _selectedInvoice, value);
                IsInvoiceEditorEnabled = value != null;
            }
        }

        private ObservableCollection<string> _expiryDaysList = new ObservableCollection<string>() { "40", "30" };
        public ObservableCollection<string> ExpiryDaysList
        {
            get => _expiryDaysList;
            set => Set(ref _expiryDaysList, value);
        }

        public string FilterText { get; set; }

        public RelayCommand AddCommand { get; set; }
        private void ExecuteAddCommand()
        {
            var newInvoice = new Invoice();
            Invoice.Add(newInvoice);
            SelectedInvoice = newInvoice;
        }

        private bool _isInvoiceEditorEnabled;
        public bool IsInvoiceEditorEnabled
        {
            get => _isInvoiceEditorEnabled;
            set => Set(ref _isInvoiceEditorEnabled, value);
        }

        public RelayCommand SaveCommand { get; set; }
        private void ExecuteSaveCommand()
        {
            _dataService.UpdateCompany(Company);
        }

        public RelayCommand DeleteCommand { get; set; }
        private void ExecuteDeleteCommand()
        {
            _dataService.RemoveInvoice(SelectedInvoice);
            Invoice.Remove(SelectedInvoice);
            Messenger.Default.Send(new NotificationMessage("TotalSaldoChanged"));
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

        public InvoiceViewModel(IDataService dataService, Company company) : base(company.Name)
        {
            _dataService = dataService;
            Company = company;
            Invoice = company.Invoice;

            AddCommand = new RelayCommand(ExecuteAddCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            DeleteCommand = new RelayCommand(ExecuteDeleteCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);
            PrintCommand = new RelayCommand<DataGrid>(ExecutePrintCommand);
            SearchCommand = new RelayCommand(ExecuteSearchCommand);
        }
    }
}