using System;
using System.Collections.ObjectModel;
using FinanceSaldo.ViewModel;

namespace FinanceSaldo.Model
{
    public interface IDataService
    {
        void GetCompany(Action<ObservableCollection<Company>, Exception> callback);
        void GetCompanyWithSaldo(Action<ObservableCollection<CompanyList>, Exception> callback);
        void CreateCompany(Company company);
        void UpdateCompany(Company company);
        void RemoveCompany(Company company);
        void RemoveInvoice(Invoice invoice);
    }
}
