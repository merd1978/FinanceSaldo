using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FinanceSaldo.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SimpleDialogs;
using SimpleDialogs.Controls;
using SimpleDialogs.Enumerators;

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

        public CollectionView InvoiceView { get; private set; }

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

        #region Filter
        private string _filterText;
        public string FilterText
        {
            get => _filterText;
            set
            {
                Set(ref _filterText, value);
                ResetFilterTextCommand.RaiseCanExecuteChanged();
                InvoiceView.Refresh();
            }
        }

        private DateTime _filterStartDate;
        public DateTime FilterStartDate
        {
            get => _filterStartDate;
            set
            {
                Set(ref _filterStartDate, value);
                RaisePropertyChanged(nameof(FilterDateDif));
                InvoiceView.Refresh();
            }
        }

        private DateTime _filterEndDate = DateTime.Now.Date;
        public DateTime FilterEndDate
        {
            get => _filterEndDate;
            set
            {
                Set(ref _filterEndDate, value);
                RaisePropertyChanged(nameof(FilterDateDif));
                InvoiceView.Refresh();
            }
        }

        public double FilterDateDif
        {
            get => (FilterEndDate.Date - FilterStartDate.Date).TotalDays;
            set
            {
                FilterStartDate = FilterEndDate.AddDays(-value);
                RaisePropertyChanged(nameof(FilterStartDate));
            }
        }

        private ObservableCollection<string> _filterDateDifList = new ObservableCollection<string>() { "10", "20", "30", "40" };
        public ObservableCollection<string> FilterDateDifList
        {
            get => _filterDateDifList;
            set => Set(ref _filterDateDifList, value);
        }

        bool OnFilterInvoice(object item)
        {
            var invoice = (Invoice)item;
            if (invoice.Date.Date < FilterStartDate.Date || invoice.Date.Date > FilterEndDate.Date) return false;

            if (string.IsNullOrEmpty(FilterText)) return true;
            if (invoice.Name == null) return false;
            return invoice.Name.Contains(FilterText);
        }
        #endregion

        #region Commands
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

        public RelayCommand DialogDeleteCommand { get; set; }
        private void ExecuteDialogDeleteCommand()
        {
            DialogManager.ShowDialog(new AlertDialog()
            {
                AlertLevel = AlertLevel.Warning,
                ButtonsStyle = DialogButtonStyle.YesNo,
                YesButtonContent = "Да",
                NoButtonContent = "Нет",
                ExitDialogCommand = DeleteCommand,
                ShowCopyToClipboardButton = false,
                Message = $"Удалить запись {SelectedInvoice.Name}?",
                Title = "Подтверждение удаления",
            });
        }

        public RelayCommand<DialogResult> DeleteCommand { get; set; }
        private void ExecuteDeleteCommand(DialogResult result)
        {
            DialogManager.HideVisibleDialog();
            if (result != DialogResult.Yes) return;
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
            if ((bool)printdlg.ShowDialog().GetValueOrDefault())
            {
                Size pageSize = new Size(printdlg.PrintableAreaWidth, printdlg.PrintableAreaHeight);
                dataGrid.Measure(pageSize);
                dataGrid.Arrange(new Rect(5, 5, pageSize.Width, pageSize.Height));
                printdlg.PrintVisual(dataGrid, "Печать отчета");
            }
        }

        public RelayCommand ResetFilterTextCommand { get; set; }
        private void ExecuteResetFilterTextCommand()
        {
            FilterText = String.Empty;
        }
        private bool CanExecuteResetFilterTextCommand()
        {
            return !string.IsNullOrEmpty(FilterText);
        }

        #endregion

        public InvoiceViewModel(IDataService dataService, Company company) : base(company.Name)
        {
            _dataService = dataService;
            Company = company;
            Invoice = company.Invoice;
            InvoiceView = (CollectionView)CollectionViewSource.GetDefaultView(Invoice);
            InvoiceView.Filter = OnFilterInvoice;
            FilterDateDif = 30;

            AddCommand = new RelayCommand(ExecuteAddCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            DialogDeleteCommand = new RelayCommand(ExecuteDialogDeleteCommand);
            DeleteCommand = new RelayCommand<DialogResult>(ExecuteDeleteCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);
            PrintCommand = new RelayCommand<DataGrid>(ExecutePrintCommand);
            ResetFilterTextCommand = new RelayCommand(ExecuteResetFilterTextCommand, CanExecuteResetFilterTextCommand);
        }
    }
}