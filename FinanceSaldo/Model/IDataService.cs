using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace FinanceSaldo.Model
{
    public interface IDataService
    {
        void GetCompany(Action<ObservableCollection<Company>, Exception> callback);
        void CreateCompany(Company company);
        void RemoveCompany(Company company);

        void GetInvoice(Action<ObservableCollection<Invoice>, Exception> callback);
    }
}
