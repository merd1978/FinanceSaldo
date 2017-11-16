using FinanceSaldo.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FinanceSaldo.ViewModel
{
    public class CompanyEditViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        public string TabName
        {
            get;
            private set;
        }

        private Company _company = new Company();
        public Company Company
        {
            get => _company;
            set => Set(ref _company, value);
        }

        public RelayCommand CreateCompanyCommand { get; set; }
        private void ExecuteCreateCompanyCommand()
        {
            _dataService.CreateCompany(_company);
            Messenger.Default.Send(new NotificationMessage("RefreshCompany"));
        }

        public RelayCommand CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand()
        {
            Messenger.Default.Send(new NotificationMessage("CloseCurrentTab"));
        }

        public CompanyEditViewModel(string tabName, IDataService dataService)
        {
            TabName = tabName;
            _dataService = dataService;
            CreateCompanyCommand = new RelayCommand(ExecuteCreateCompanyCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);
        }

    }
}