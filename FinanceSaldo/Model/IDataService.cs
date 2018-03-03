using System;
using System.Collections.ObjectModel;

namespace FinanceSaldo.Model
{
    public interface IDataService
    {
        void GetCompany(Action<ObservableCollection<Company>, Exception> callback);
        void InsertOrUpdateCompany(Company company);
        void SaveChanges();
        bool HasUnsavedChanges();
        void RemoveCompany(Company company, Action<Exception> callback);
        void RemoveInvoice(Invoice invoice);
    }
}
