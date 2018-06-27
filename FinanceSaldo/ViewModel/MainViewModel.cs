using System;
using GalaSoft.MvvmLight;
using FinanceSaldo.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using SimpleDialogs;
using SimpleDialogs.Controls;
using SimpleDialogs.Enumerators;

namespace FinanceSaldo.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        #region Commands

        public RelayCommand WindowLoadedCommand { get; set; }
        private void ExecuteWindowLoadedCommand()
        {
            //DialogManager.ShowDialog(new ProgressDialog()
            //{
            //    CanCancel = false,
            //    IsUndefined = true,
            //    Message = $"Ждите....",
            //    Title = "Загрузка данных",
            //});

            //var task = GetStudent();
            //task.Wait();
            //var result = task.Result;

            GetCompany();
        }

        public RelayCommand EditCompanyCommand { get; set; }
        private bool CanExecuteEditCompanyCommand()
        {
            return SelectedItem != null;
        }
        private void ExecuteEditCompanyCommand()
        {
            TabCollection.Add(new CompanyEditViewModel(_dataService, SelectedItem));
            SelectedTabIndex = TabCollection.Count - 1;
        }

        public RelayCommand NewCompanyCommand { get; set; }
        private void ExecuteNewCompanyCommand()
        {
            TabCollection.Add(new CompanyEditViewModel(_dataService, new Company()));
            SelectedTabIndex = TabCollection.Count - 1;
        }

        public RelayCommand HelpCommand { get; set; }
        private void ExecuteHelpCommand()
        {
            TabCollection.Add(new HelpViewModel());
            SelectedTabIndex = TabCollection.Count - 1;
        }

        public RelayCommand DialogRemoveCompanyCommand { get; set; }
        private bool CanExecuteDialogRemoveCompanyCommand()
        {
            return SelectedItem != null;
        }
        private void ExecuteDialogRemoveCompanyCommand()
        {
            if (SelectedItem == null) return;

            DialogManager.ShowDialog(new AlertDialog()
            {
                AlertLevel = AlertLevel.Warning,
                ButtonsStyle = DialogButtonStyle.YesNo,
                YesButtonContent = "Да",
                NoButtonContent = "Нет",
                ExitDialogCommand = RemoveCompanyCommand,
                ShowCopyToClipboardButton = false,
                Message = $"Удалить {SelectedItem.Name}?",
                Title = "Подтверждение удаления",
            });
        }

        public RelayCommand<DialogResult> RemoveCompanyCommand { get; set; }
        private void ExecuteRemoveCompanyCommand(DialogResult result)
        {
            DialogManager.HideVisibleDialog();
            if (result != DialogResult.Yes) return;
            Company company = SelectedItem;
            int index = SelectedIndex;
            RemoveCompany(company);
            TabCollection.Remove(company.InvoiceViewModel);
            Company.RemoveAt(index);
        }

        public RelayCommand<TabViewModelBase> CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand(TabViewModelBase viewModel)
        {
            if (viewModel is InvoiceViewModel invoiceViewModel)
            {
                int indx = Company.IndexOf(invoiceViewModel.Company);
                if (indx == -1)
                {
                    //error company not found
                    return;
                }
                Company[indx].InvoiceViewModel = null;
            }
            TabCollection.Remove(viewModel);
        }

        public RelayCommand OpenCompanyTabCommand { get; set; }
        private void ExecuteOpenCompanyTabCommand()
        {
            InvoiceViewModel invoiceViewModel = Company[SelectedIndex].InvoiceViewModel;
            if (invoiceViewModel == null)
            {
                invoiceViewModel = new InvoiceViewModel(_dataService, SelectedItem);
                TabCollection.Add(invoiceViewModel);
                Company[SelectedIndex].InvoiceViewModel = invoiceViewModel;
                SelectedTabIndex = TabCollection.Count - 1;
            }
            else
            {
                SelectedTabIndex = TabCollection.IndexOf(invoiceViewModel);
            }
        }

        public RelayCommand<CancelEventArgs> DialogWindowCloseCommand { get; set; }
        private void ExecuteDialogWindowCloseCommand(CancelEventArgs cancelEventArgs)
        {
            if (_dataService.HasUnsavedChanges())
            {
                DialogManager.ShowDialog(new AlertDialog()
                {
                    AlertLevel = AlertLevel.Warning,
                    ButtonsStyle = DialogButtonStyle.YesNo,
                    YesButtonContent = "Да",
                    NoButtonContent = "Нет",
                    ExitDialogCommand = WindowCloseCommand,
                    ShowCopyToClipboardButton = false,
                    Message = $"Вы хотите сохранить изменения?",
                    Title = "Выход из приложения",
                });
                cancelEventArgs.Cancel = true;
            }
        }

        public RelayCommand<DialogResult> WindowCloseCommand { get; set; }
        private void ExecuteWindowCloseCommand(DialogResult dialogResult)
        {
            DialogManager.HideVisibleDialog();
            if (dialogResult == DialogResult.Yes) _dataService.SaveChanges();
            Application.Current.Shutdown();
        }
        #endregion

        ObservableCollection<Company> _company;
        public ObservableCollection<Company> Company
        {
            get => _company;
            set => Set(ref _company, value);
        }

        private Company _selectedItem;
        public Company SelectedItem
        {
            get => _selectedItem;
            set
            {
                Set(ref _selectedItem, value);
                if (value != null) ExecuteOpenCompanyTabCommand();
                DialogRemoveCompanyCommand.RaiseCanExecuteChanged();
                EditCompanyCommand.RaiseCanExecuteChanged();
            } 
        }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => Set(ref _selectedIndex, value);
        }

        public ObservableCollection<TabViewModelBase> TabCollection { get; } = new ObservableCollection<TabViewModelBase>();
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set
            {
                Set(ref _selectedTabIndex, value);
                if (value == -1)
                {
                    SelectedIndex = -1;
                    return;
                }

                if (TabCollection[value] is InvoiceViewModel invoiceViewModel)
                {
                    SelectedIndex = Company.IndexOf(invoiceViewModel.Company);
                    invoiceViewModel.SelectedInvoice = invoiceViewModel.SelectedInvoice;
                }
                else
                {
                    SelectedIndex = -1;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            _dataService = dataService;
            
            EditCompanyCommand = new RelayCommand(ExecuteEditCompanyCommand, CanExecuteEditCompanyCommand);
            NewCompanyCommand = new RelayCommand(ExecuteNewCompanyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            DialogRemoveCompanyCommand = new RelayCommand(ExecuteDialogRemoveCompanyCommand, CanExecuteDialogRemoveCompanyCommand);
            RemoveCompanyCommand = new RelayCommand<DialogResult>(ExecuteRemoveCompanyCommand);
            CloseTabCommand = new RelayCommand<TabViewModelBase>(ExecuteCloseTabCommand);
            OpenCompanyTabCommand = new RelayCommand(ExecuteOpenCompanyTabCommand);
            DialogWindowCloseCommand = new RelayCommand<CancelEventArgs>(ExecuteDialogWindowCloseCommand);
            WindowCloseCommand = new RelayCommand<DialogResult>(ExecuteWindowCloseCommand);
            WindowLoadedCommand = new RelayCommand(ExecuteWindowLoadedCommand);

            Messenger.Default.Register<NotificationMessage>(this, NotifyMe);
            Messenger.Default.Register<Company>(this, AddCompany);
        }

        public void AddCompany(Company company)
        {
            Company.Add(company);
        }

        public void NotifyMe(NotificationMessage notificationMessage)
        {
            string notification = notificationMessage.Notification;
            switch (notification)
            {
                case "CloseCurrentTab":
                    ExecuteCloseTabCommand(TabCollection[SelectedTabIndex]);
                    break;
                case "TotalSaldoChanged":
                    SelectedItem.RaisePropertyChanged(nameof(SelectedItem.TotalSaldo));
                    break;
            }
        }

        public void GetCompany()
        {
            Company = new ObservableCollection<Company>();
            _dataService.GetCompany((items, error) =>
            {
                if (error != null)
                {
                    // Get Company list error
                    return;
                }
                Company = items;
            });
        }

        public void GetCompanyAsync()
        {
            //var task = _dataService.GetCompanyAsync();
            //task.Wait();
            //Company = new ObservableCollection<Company>(task.Result);

            var task = GetStudent();
            task.Wait();
            var result = task.Result;
        }

        private static async Task<Company> GetStudent()
        {
            Company company = null;

            using (var context = new DataEntity())
            {
                company = await (context.Company.Where(s => s.CompanyId == 1).FirstOrDefaultAsync());
            }

            return company;
        }

        public void RemoveCompany(Company company)
        {
            _dataService.RemoveCompany(company, (error) =>
            {
                if (error != null)
                {
                    // Unable remove company
                    return;
                }
            });
        }

        //public override void Cleanup()
        //{
        //    // Clean up if needed

        //    base.Cleanup();
        //}
    }
}