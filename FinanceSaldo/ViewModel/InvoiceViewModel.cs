using System.Collections.ObjectModel;
using FinanceSaldo.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FinanceSaldo.ViewModel
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        public string TabName
        {
            get;
            private set;
        }

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

        public RelayCommand UpdateCompanyCommand { get; set; }
        private void ExecuteUpdateCompanyCommand()
        {
            Company.Invoice = Invoice;
            _dataService.UpdateCompany(Company);
            //Messenger.Default.Send(new NotificationMessage("UpdateCompany"));
        }

        public RelayCommand CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand()
        {
            Messenger.Default.Send(new NotificationMessage("CloseCurrentTab"));
        }

        public InvoiceViewModel(string tabName, IDataService dataService, Company company)
        {
            TabName = tabName;
            _dataService = dataService;
            Company = company;
            Invoice = new ObservableCollection<Invoice>(company.Invoice);

            UpdateCompanyCommand = new RelayCommand(ExecuteUpdateCompanyCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);
        }
    }
}