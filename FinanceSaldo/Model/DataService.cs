using System;
using System.Collections.ObjectModel;

namespace FinanceSaldo.Model
{
    public class DataService : IDataService
    {
        DataEntity context;
        public DataService()
        {
            context = new DataEntity();
            context.Database.CreateIfNotExists();
        }
        public void GetCompany(Action<ObservableCollection<Company>, Exception> callback)
        {
            var _company = context.Company;
            callback(new ObservableCollection<Company>(_company), null);
        }
        public void GetInvoice(Action<ObservableCollection<Invoice>, Exception> callback)
        {
            var _invoice = context.Invoice;
            callback(new ObservableCollection<Invoice>(_invoice), null);
        }
    }
}