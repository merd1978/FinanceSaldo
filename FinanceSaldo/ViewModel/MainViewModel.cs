using GalaSoft.MvvmLight;
using FinanceSaldo.Model;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

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
            MessageBoxResult messageBoxResult = MessageBox.Show("Удалить " + SelectedItem.Company.Name + "?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                CompanyList item = SelectedItem;
                int index = SelectedIndex;
                RemoveCompany(item.Company);
                TabCollection.Remove(item.InvoiceViewModel);
                CompanyList.RemoveAt(index);
            }
        }

        public RelayCommand<TabViewModelBase> CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand(TabViewModelBase viewModel)
        {
            if (viewModel is InvoiceViewModel invoiceViewModel)
            {
                int indx = CompanyList.IndexOf(invoiceViewModel.CompanyList);
                if (indx == -1)
                {
                    //error company not found
                    return;
                }
                CompanyList[indx].InvoiceViewModel = null;
            }
            TabCollection.Remove(viewModel);
        }

        public RelayCommand OpenCompanyTabCommand { get; set; }
        private void ExecuteOpenCompanyTabCommand()
        {
            InvoiceViewModel invoiceViewModel = CompanyList[SelectedIndex].InvoiceViewModel;
            if (invoiceViewModel == null)
            {
                invoiceViewModel = new InvoiceViewModel(_dataService, SelectedItem);
                TabCollection.Add(invoiceViewModel);
                CompanyList[SelectedIndex].InvoiceViewModel = invoiceViewModel;
                SelectedTabIndex = TabCollection.Count - 1;
            }
            else
            {
                SelectedTabIndex = TabCollection.IndexOf(invoiceViewModel);
            }
        }
        #endregion

        ObservableCollection<CompanyList> _companyList;
        public ObservableCollection<CompanyList> CompanyList
        {
            get => _companyList;
            set => Set(ref _companyList, value);
        }

        private CompanyList _selectedItem;
        public CompanyList SelectedItem
        {
            get => _selectedItem;
            set
            {
                Set(ref _selectedItem, value);
                if (value != null) ExecuteOpenCompanyTabCommand();
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
                    SelectedIndex = CompanyList.IndexOf(invoiceViewModel.CompanyList);
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
            RemoveCompanyCommand = new RelayCommand(ExecuteRemoveCompanyCommand);
            CloseTabCommand = new RelayCommand<TabViewModelBase>(ExecuteCloseTabCommand);
            OpenCompanyTabCommand = new RelayCommand(ExecuteOpenCompanyTabCommand);

            Messenger.Default.Register<NotificationMessage>(this, NotifyMe);
        }

        public void NotifyMe(NotificationMessage notificationMessage)
        {
            string notification = notificationMessage.Notification;
            switch (notification)
            {
                case "UpdateCompany":
                    GetCompany();
                    break;
                case "CloseCurrentTab":
                    ExecuteCloseTabCommand(TabCollection[SelectedTabIndex]);
                    break;
            }
        }

        public void GetCompany()
        {
            CompanyList = new ObservableCollection<CompanyList>();
            _dataService.GetCompanyWithSaldo((items, error) =>
            {
                if (error != null)
                {
                    // Get Company list error
                    return;
                }
                CompanyList = items;
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