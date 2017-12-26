using System;
using System.Collections.ObjectModel;
using FinanceSaldo.ViewModel;

namespace FinanceSaldo.Model
{
    public interface IDataService
    {
        void GetCompany(Action<ObservableCollection<CompanyList>, Exception> callback);
        void CreateCompany(Company company);
        void UpdateCompany(Company company);
        void RemoveCompany(Company company, Action<Exception> callback);
        void RemoveInvoice(Invoice invoice);
    }
}
