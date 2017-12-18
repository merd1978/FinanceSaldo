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
            if (SelectedCompany == null) return;
            MessageBoxResult messageBoxResult = MessageBox.Show("Удалить " + SelectedCompany.Company.Name + "?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _dataService.RemoveCompany(SelectedCompany.Company);
                GetCompany();
            }
        }

        public RelayCommand<ViewModelBase> CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand(ViewModelBase viewModel)
        {
            TabCollection.Remove(viewModel);
        }

        public RelayCommand<CompanyList> OpenCompanyTabCommand { get; set; }
        private void ExecuteOpenCompanyTabCommand(CompanyList companyList)
        {
            TabCollection.Add(new InvoiceViewModel(companyList.Company.Name, _dataService, companyList.Company));
            SelectedTabIndex = TabCollection.Count - 1;
        }
        #endregion

        ObservableCollection<CompanyList> _companyList;
        public ObservableCollection<CompanyList> CompanyList
        {
            get => _companyList;
            set => Set(ref _companyList, value);
        }

        private CompanyList _selectedCompany;
        public CompanyList SelectedCompany
        {
            get => _selectedCompany;
            set => Set(ref _selectedCompany, value);
        }

        public ObservableCollection<ViewModelBase> TabCollection { get; } = new ObservableCollection<ViewModelBase>();
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => Set(ref _selectedTabIndex, value);
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
            CloseTabCommand = new RelayCommand<ViewModelBase>(ExecuteCloseTabCommand);
            OpenCompanyTabCommand = new RelayCommand<CompanyList>(ExecuteOpenCompanyTabCommand);

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