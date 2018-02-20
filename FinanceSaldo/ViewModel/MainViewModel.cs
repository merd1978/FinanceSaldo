using GalaSoft.MvvmLight;
using FinanceSaldo.Model;
using System.Collections.ObjectModel;
using System.Globalization;
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
        public RelayCommand EditCompanyCommand { get; set; }
        private void ExecuteEditCompanyCommand()
        {
            //CurrentView = View.CompanyView;
        }

        public RelayCommand NewCompanyCommand { get; set; }
        private void ExecuteNewCompanyCommand()
        {
            TabCollection.Add(new CompanyEditViewModel(_dataService));
            SelectedTabIndex = TabCollection.Count - 1;
        }

        public RelayCommand HelpCommand { get; set; }
        private void ExecuteHelpCommand()
        {
            TabCollection.Add(new HelpViewModel());
            SelectedTabIndex = TabCollection.Count - 1;
        }

        public RelayCommand RemoveCompanyCommand { get; set; }
        private void ExecuteRemoveCompanyCommand()
        {
            if (SelectedItem == null) return;

            DialogManager.ShowDialog(new AlertDialog()
            {
                AlertLevel = AlertLevel.Warning,
                ButtonsStyle = DialogButtonStyle.YesNo,
                YesButtonContent = "Да",
                NoButtonContent = "Нет",
                //ExitDialogCommand = DialogCloseCommand,
                ShowCopyToClipboardButton = false,
                Message = $"Удалить {SelectedItem.Name}?",
                Title = "Подтверждение удаления"
            });

            //if (dialogResult == MessageBoxResult.Yes)
            //{
            //    //Company item = SelectedItem;
            //    //int index = SelectedIndex;
            //    //RemoveCompany(item);
            //    //TabCollection.Remove(item.InvoiceViewModel);
            //    //Company.RemoveAt(index);
            //}
        }
        private bool CanExecuteRemoveCompanyCommand()
        {
            return SelectedItem != null;
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
                RemoveCompanyCommand.RaiseCanExecuteChanged();
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
            GetCompany();

            EditCompanyCommand = new RelayCommand(ExecuteEditCompanyCommand);
            NewCompanyCommand = new RelayCommand(ExecuteNewCompanyCommand);
            HelpCommand = new RelayCommand(ExecuteHelpCommand);
            RemoveCompanyCommand = new RelayCommand(ExecuteRemoveCompanyCommand, CanExecuteRemoveCompanyCommand);
            CloseTabCommand = new RelayCommand<TabViewModelBase>(ExecuteCloseTabCommand);
            OpenCompanyTabCommand = new RelayCommand(ExecuteOpenCompanyTabCommand);

            Messenger.Default.Register<NotificationMessage>(this, NotifyMe);
            Messenger.Default.Register<Company>(this, AddCompany);
        }

        public void AddCompany(Company company)
        {
            Company.Add(new Company());
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

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}