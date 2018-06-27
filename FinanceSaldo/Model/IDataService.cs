using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FinanceSaldo.Model
{
    public interface IDataService
    {
        void GetCompany(Action<ObservableCollection<Company>, Exception> callback);
        Task<List<Company>> GetCompanyAsync();
        void InsertOrUpdateCompany(Company company);
        void SaveChanges();
        bool HasUnsavedChanges();
        void RemoveCompany(Company company, Action<Exception> callback);
        void RemoveInvoice(Invoice invoice);
    }
}
