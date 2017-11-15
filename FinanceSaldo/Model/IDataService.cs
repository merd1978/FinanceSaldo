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
        void GetInvoice(Action<ObservableCollection<Invoice>, Exception> callback);
        void CreateCompany(Company company);
    }
}
