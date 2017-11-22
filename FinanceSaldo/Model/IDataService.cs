using System;
using System.Collections.ObjectModel;

namespace FinanceSaldo.Model
{
    public interface IDataService
    {
        void GetCompany(Action<ObservableCollection<Company>, Exception> callback);
        void CreateCompany(Company company);
        void UpdateCompany(Company company);
        void RemoveCompany(Company company);
    }
}
