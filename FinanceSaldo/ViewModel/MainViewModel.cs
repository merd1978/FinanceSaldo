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

        public RelayCommand RemoveCompanyCommand { get; set; }
        private void ExecuteRemoveCompanyCommand()
        {
            if (SelectedCompany == null) return;
            MessageBoxResult messageBoxResult = MessageBox.Show("Удалить " + SelectedCompany.Name + "?", "Подтверждение удаления",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _dataService.RemoveCompany(SelectedCompany);
                GetCompany();
            }
        }

        public RelayCommand<ViewModelBase> CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand(ViewModelBase viewModel)
        {
            TabCollection.Remove(viewModel);
        }

        public RelayCommand<Company> OpenCompanyTabCommand { get; set; }
        private void ExecuteOpenCompanyTabCommand(Company company)
        {
            TabCollection.Add(new InvoiceViewModel(company.Name, _dataService, company));
            SelectedTabIndex = TabCollection.Count - 1;
        }
        #endregion

        ObservableCollection<Company> _company;
        public ObservableCollection<Company> Company
        {
            get => _company;
            set => Set(ref _company, value);
        }

        private Company _selectedCompany;
        public Company SelectedCompany
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
            //Environment.SetEnvironmentVariable("USERPROFILE", "e:\\kursovoy");

            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            _dataService = dataService;
            GetCompany();
            
            EditCompanyCommand = new RelayCommand(ExecuteEditCompanyCommand);
            NewCompanyCommand = new RelayCommand(ExecuteNewCompanyCommand);
            RemoveCompanyCommand = new RelayCommand(ExecuteRemoveCompanyCommand);
            CloseTabCommand = new RelayCommand<ViewModelBase>(ExecuteCloseTabCommand);
            OpenCompanyTabCommand = new RelayCommand<Company>(ExecuteOpenCompanyTabCommand);

            Messenger.Default.Register<NotificationMessage>(this, NotifyMe);
        }

        public void NotifyMe(NotificationMessage notificationMessage)
        {
            string notification = notificationMessage.Notification;
            switch (notification)
            {
                case "RefreshCompany":
                    GetCompany();
                    break;
                case "CloseCurrentTab":
                    ExecuteCloseTabCommand(TabCollection[SelectedTabIndex]);
                    break;
            }
        }

        public void GetCompany()
        {
            Company = new ObservableCollection<Company>();
            _dataService.GetCompany(
                (items, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    Company = items;
                });
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}