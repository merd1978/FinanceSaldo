using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FinanceSaldo.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SimpleDialogs;
using SimpleDialogs.Controls;
using SimpleDialogs.Enumerators;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace FinanceSaldo.ViewModel
{
    public class InvoiceViewModel : TabViewModelBase, IDataErrorInfo
    {
        private readonly IDataService _dataService;

        private Company _сompany;
        public Company Company
        {
            get => _сompany;
            set => Set(ref _сompany, value);
        }

        private ObservableCollection<Invoice> _invoice;
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

        private int _invoiceExpiryDays;
        [Range(0, 999, ErrorMessage = "Срок должен быть от 1 до 999")]
        public int InvoiceExpiryDays
        {
            get => _invoiceExpiryDays;
            set
            {
                Set(ref _invoiceExpiryDays, value);

                SelectedInvoice.ExpiryDays = value;
                RaisePropertyChanged(() => SelectedInvoice.ExpiryDays);
            }
        }

        private decimal _currentSaldo;
        public decimal CurrentSaldo
        {
            get => Company.GetSaldo(FilterEndDate);
            set => Set(ref _currentSaldo, value);
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
                RaisePropertyChanged(nameof(CurrentSaldo));
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
        public RelayCommand NewCommand { get; set; }
        private void ExecuteNewCommand()
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
            _dataService.SaveChanges();
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

            NewCommand = new RelayCommand(ExecuteNewCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            DialogDeleteCommand = new RelayCommand(ExecuteDialogDeleteCommand);
            DeleteCommand = new RelayCommand<DialogResult>(ExecuteDeleteCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);
            PrintCommand = new RelayCommand<DataGrid>(ExecutePrintCommand);
            ResetFilterTextCommand = new RelayCommand(ExecuteResetFilterTextCommand, CanExecuteResetFilterTextCommand);

            Messenger.Default.Register<NotificationMessage>(this, NotifyMe);
        }

        public void NotifyMe(NotificationMessage notificationMessage)
        {
            string notification = notificationMessage.Notification;
            switch (notification)
            {
                case "CurrentSaldoChanged":
                    RaisePropertyChanged(nameof(CurrentSaldo));
                    break;
            }
        }

        public string this[string columnName]
        {
            get
            {
                //string result = string.Empty;
                //switch (columnName)
                //{
                //    case "Name": if (string.IsNullOrEmpty(Company.Name)) result = "Name is required!"; break;
                //};
                //return result;

                var value = GetType().GetProperty(columnName)?.GetValue(this, null);
                var results = new List<ValidationResult>();

                var context = new ValidationContext(this, null, null) { MemberName = columnName };

                if (!Validator.TryValidateProperty(value, context, results))
                {
                    return results.First().ErrorMessage;
                }

                return string.Empty;
            }
        }

        public string Error => throw new NotImplementedException();
    }
}