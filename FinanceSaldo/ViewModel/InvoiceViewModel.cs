using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using FinanceSaldo.Model;
using FinanceSaldo.View.Extensions;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SimpleDialogs;
using SimpleDialogs.Controls;
using SimpleDialogs.Enumerators;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using Excel = Microsoft.Office.Interop.Excel;

namespace FinanceSaldo.ViewModel
{
    public sealed class InvoiceViewModel : TabViewModelBase, IDataErrorInfo
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

        public ICollectionView InvoiceView { get; private set; }

        private Invoice _selectedInvoice;
        public Invoice SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                Set(ref _selectedInvoice, value);
                IsInvoiceEditorEnabled = value != null;
                RaisePropertyChanged(nameof(InvoiceName));
                RaisePropertyChanged(nameof(InvoiceDebit));
                RaisePropertyChanged(nameof(InvoiceCredit));
                if (value != null)
                {
                    _invoiceExpiryDays = SelectedInvoice.ExpiryDays;
                    RaisePropertyChanged(nameof(InvoiceExpiryDays));
                }

                InvoiceDate = value?.Date ?? DateTime.Now;
            }
        }

        public string InvoiceName
        {
            get
            {
                if (SelectedInvoice != null) return SelectedInvoice.Name;
                return string.Empty;
            }
            set => SelectedInvoice.Name = value;
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
                TotalSumChanged();
                TotalSaldoChanged();
            }
        }

        private DateTime _invoiceDate;
        public DateTime InvoiceDate
        {
            get => SelectedInvoice != null ? _invoiceDate : DateTime.Now;
            set
            {
                Set(ref _invoiceDate, value);
                if (value < FilterStartDate.Date || value > FilterEndDate) return;
                SelectedInvoice.Date = value;
                TotalSaldoChanged();
            }
        }

        public decimal InvoiceDebit
        {
            get
            {
                if (SelectedInvoice != null) return SelectedInvoice.Debit;
                return Decimal.Zero;
            }
            set
            {
                SelectedInvoice.Debit = value;
                TotalSumChanged();
                TotalSaldoChanged();
            }
        }

        public decimal InvoiceCredit
        {
            get
            {
                if (SelectedInvoice != null) return SelectedInvoice.Credit;
                return Decimal.Zero;
            }
            set
            {
                SelectedInvoice.Credit = value;
                TotalSumChanged();
                TotalSaldoChanged();
            }
        }

        // Summ of filtered Debit (ExpiryDays != 0)
        public decimal TotalDebit
        {
            get
            {
                decimal sum = 0;
                foreach (Invoice inv in InvoiceView)
                {
                    if (inv.ExpiryDays != 0) sum += inv.Debit;
                }
                return sum;
            }
        }

        // Summ of filtered Debit for Cash (ExpiryDays == 0)
        public decimal TotalCashDebit
        {
            get
            {
                decimal sum = 0;
                foreach (Invoice inv in InvoiceView)
                {
                    if (inv.ExpiryDays == 0) sum += inv.Debit;
                }
                return sum;
            }
        }

        // Summ of filtered Debit (ExpiryDays != 0)
        public decimal TotalCredit
        {
            get
            {
                decimal sum = 0;
                foreach (Invoice inv in InvoiceView)
                {
                    if (inv.ExpiryDays != 0) sum += inv.Credit;
                }
                return sum;
            }
        }

        // Summ of filtered Debit for Cash (ExpiryDays == 0)
        public decimal TotalCashCredit
        {
            get
            {
                decimal sum = 0;
                foreach (Invoice inv in InvoiceView)
                {
                    if (inv.ExpiryDays == 0) sum += inv.Credit;
                }
                return sum;
            }
        }

        private decimal _currentSaldo;
        public decimal CurrentSaldo
        {
            get => Company.GetSaldo(FilterEndDate);
            set => Set(ref _currentSaldo, value);
        }

        private decimal _expiredSaldo;
        public decimal ExpiredSaldo
        {
            get => Company.GetExpiredSaldo(FilterEndDate);
            set => Set(ref _expiredSaldo, value);
        }

        private ObservableCollection<string> _expiryDaysList = new ObservableCollection<string>() { "0", "30", "40" };
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
                TotalSumChanged();
            }
        }

        private DateTime _filterStartDate = DateTime.Now.Date;
        public DateTime FilterStartDate
        {
            get => _filterStartDate;
            set
            {
                Set(ref _filterStartDate, value);
                RaisePropertyChanged(nameof(FilterDateDif));
                InvoiceView.Refresh();
                TotalSumChanged();
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
                TotalSaldoChanged();
                TotalSumChanged();
            }
        }

        [Range(1, 999, ErrorMessage = "Срок должен быть от 1 до 999")]
        public int FilterDateDif
        {
            get => (int)(FilterEndDate.Date - FilterStartDate.Date).TotalDays + 1;
            set
            {
                if (value < 1 || value > 999) value = 0;
                FilterStartDate = FilterEndDate.AddDays(-(value - 1));
                RaisePropertyChanged(nameof(FilterStartDate));
                TotalSumChanged();
            }
        }

        private ObservableCollection<string> _filterDateDifList = new ObservableCollection<string>() { "10", "20", "30", "40", "90" };
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
        #endregion Filter

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

        public RelayCommand<DatePickerDateValidationErrorEventArgs> DatePickerConverterCommand { get; set; }
        private void ExecuteDatePickerConverterCommand(DatePickerDateValidationErrorEventArgs e)
        {
            var converter = new DateConverter();
            if (converter.ConvertBack(e.Text, typeof(DateTime?), null, CultureInfo.CurrentCulture) is DateTime value)
                InvoiceDate = value;
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

        public RelayCommand Export2ExcelCommand { get; set; }
        private void ExecuteExport2ExcelCommand()
        {
            // Load Excel application
            Excel.Application excel = new Excel.Application();

            // Create empty workbook
            excel.Workbooks.Add();

            // Create Worksheet from active sheet
            Excel._Worksheet workSheet = excel.ActiveSheet;

            // I created Application and Worksheet objects before try/catch,
            // so that i can close them in finnaly block.
            // It's IMPORTANT to release these COM objects!!
            try
            {
                // Creation of header cells
                workSheet.Cells[1, "A"] = $"Сальдо по предприятию: {Company.Name}";
                workSheet.Cells[2, "A"] = "№ Накладной";
                workSheet.Cells[2, "B"] = "Дата отгрузки";
                workSheet.Cells[2, "C"] = "Срок погашения";
                workSheet.Cells[2, "D"] = "Дата погашения";
                workSheet.Cells[2, "E"] = "Дебит";
                workSheet.Cells[2, "F"] = "Кредит";

                // Populate sheet with some real data
                int row = 3; // start row
                foreach (Invoice invoice in (IEnumerable)InvoiceView)
                {
                    workSheet.Cells[row, "A"] = invoice.Name;
                    workSheet.Cells[row, "B"] = invoice.Date.Date;
                    workSheet.Cells[row, "C"] = invoice.ExpiryDays;
                    workSheet.Cells[row, "D"] = invoice.ExpiryDate.Date;
                    workSheet.Cells[row, "E"] = invoice.Debit;
                    workSheet.Cells[row, "F"] = invoice.Credit;
                    row++;
                }
                // Creation of summary cells
                row++;
                workSheet.Cells[row++, "A"] = $"Просроченное сальдо (на {FilterEndDate.Date.ToShortDateString()}) = {ExpiredSaldo}";
                workSheet.Cells[row, "A"] = $"Сальдо (на {FilterEndDate.Date.ToShortDateString()}) = {CurrentSaldo}";

                // Apply some predefined styles for data to look nicely :)
                workSheet.Range["A1"].AutoFormat();

                excel.Visible = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show("Exception",
                    "There was a PROBLEM saving Excel file!\n" + exception.Message, MessageBoxButton.OK, MessageBoxImage.Error);

            }
            finally
            {
                // Release COM objects (very important!)
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excel);

                if (workSheet != null) System.Runtime.InteropServices.Marshal.ReleaseComObject(workSheet);

                excel = null;
                workSheet = null;

                // Force garbage collector cleaning
                GC.Collect();
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

            InvoiceView = CollectionViewSource.GetDefaultView(Invoice);
            InvoiceView.Filter = OnFilterInvoice;

            InvoiceView.SortDescriptions.Add(new SortDescription(nameof(Model.Invoice.Date), ListSortDirection.Ascending));

            if (InvoiceView is ListCollectionView invoiceListView) invoiceListView.CustomSort = new CustomerSorter();

            if (InvoiceView is ICollectionViewLiveShaping invoiceLiveShaping && invoiceLiveShaping.CanChangeLiveFiltering)
            {
                invoiceLiveShaping.LiveFilteringProperties.Add(nameof(Model.Invoice.Name));
                invoiceLiveShaping.LiveFilteringProperties.Add(nameof(Model.Invoice.Date));
                invoiceLiveShaping.IsLiveFiltering = true;
                if (invoiceLiveShaping.CanChangeLiveSorting)
                {
                    invoiceLiveShaping.LiveSortingProperties.Add(nameof(Model.Invoice.Date));
                    invoiceLiveShaping.IsLiveSorting = true;
                }
            }

            FilterDateDif = 30;

            foreach (var invoice in Invoice)
            {
                invoice.InvoiceSaldoChanged += InvoiceSaldoChanged;
            }

            NewCommand = new RelayCommand(ExecuteNewCommand);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            DatePickerConverterCommand = new RelayCommand<DatePickerDateValidationErrorEventArgs>(ExecuteDatePickerConverterCommand);
            DialogDeleteCommand = new RelayCommand(ExecuteDialogDeleteCommand);
            DeleteCommand = new RelayCommand<DialogResult>(ExecuteDeleteCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);
            Export2ExcelCommand = new RelayCommand(ExecuteExport2ExcelCommand);
            ResetFilterTextCommand = new RelayCommand(ExecuteResetFilterTextCommand, CanExecuteResetFilterTextCommand);

            Messenger.Default.Register<NotificationMessage>(this, NotifyMe);
        }

        public void NotifyMe(NotificationMessage notificationMessage)
        {
            string notification = notificationMessage.Notification;
            switch (notification)
            {
                case "BaseSaldoChanged":
                    RaisePropertyChanged(nameof(CurrentSaldo));
                    break;
            }
        }

        private void TotalSumChanged()
        {
            RaisePropertyChanged(nameof(TotalDebit));
            RaisePropertyChanged(nameof(TotalCredit));
            RaisePropertyChanged(nameof(TotalCashDebit));
            RaisePropertyChanged(nameof(TotalCashCredit));
        }

        private void TotalSaldoChanged()
        {
            RaisePropertyChanged(nameof(CurrentSaldo));
            RaisePropertyChanged(nameof(ExpiredSaldo));
        }

        private void InvoiceSaldoChanged(object sender, PropertyChangedEventArgs e)
        {
            Messenger.Default.Send(new NotificationMessage("TotalSaldoChanged"));
        }

        public string this[string columnName]
        {
            get
            {
                 switch (columnName)
                 {
                    case "InvoiceDate":
                        if (InvoiceDate.Date < FilterStartDate.Date || InvoiceDate.Date > FilterEndDate) return "Date is out of filter range!";
                        return string.Empty;
                };

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

        public string Error => null;

        public class CustomerSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                var invoiceX = x as Invoice;
                var invoiceY = y as Invoice;
                return invoiceX.Date.CompareTo(invoiceY.Date);
            }
        }
    }
}