using GalaSoft.MvvmLight;
using FinanceSaldo.Model;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
            TabCollection.Add(new CompanyEditViewModel("Новая", _dataService));
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
                _dataService.RemoveCompany(SelectedItem.Company);
                GetCompany();
            }
        }

        public RelayCommand<TabViewModelBase> CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand(TabViewModelBase viewModel)
        {
            TabCollection.Remove(viewModel);
  
        }

        public RelayCommand OpenCompanyTabCommand { get; set; }
        private void ExecuteOpenCompanyTabCommand()
        {
            var tab = TabCollection.FirstOrDefault(t => t.TabName == SelectedItem.Company.Name);
            if (tab == null)
            {
                TabCollection.Add(new InvoiceViewModel(SelectedItem.Company.Name, _dataService, SelectedItem.Company));
                SelectedTabIndex = TabCollection.Count - 1;
            }
            else
            {
                SelectedTabIndex = TabCollection.IndexOf(tab);
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

                if (TabCollection[SelectedTabIndex] is InvoiceViewModel invoiceViewModel)
                {
                    var company = CompanyList.FirstOrDefault(c => c.Company.CompanyId == invoiceViewModel.Company.CompanyId);
                    SelectedIndex = CompanyList.IndexOf(company);
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
            _dataService.GetCompanyWithSaldo(
                (items, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }
                    CompanyList = items;
                });
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}