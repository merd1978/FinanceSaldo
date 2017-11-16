using System.Collections.ObjectModel;
using FinanceSaldo.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

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

        public InvoiceViewModel(string tabName, IDataService dataService, Company company)
        {
            TabName = tabName;
            _dataService = dataService;
            Company = company;

            GetInvoice4Company(company);
        }

        public void GetInvoice4Company(Company company)
        {
            Invoice = new ObservableCollection<Invoice>();
            _dataService.GetInvoice4Company(
                (items, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    Invoice = items;
                }, company);
        }
    }
}