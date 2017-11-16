using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FinanceSaldo.Model
{
    public class DataService : IDataService
    {
        readonly DataEntity context;
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
        public void CreateCompany(Company company)
        {
            context.Company.Add(company);
            context.SaveChanges();
        }
        public void RemoveCompany(Company company)
        {
            context.Company.Remove(company);
            context.SaveChanges();
        }

        public void GetInvoice4Company(Action<ObservableCollection<Invoice>, Exception> callback, Company company)
        {
            var invoice = context.Invoice.Where(p => p.Company.CompanyId == company.CompanyId);
            callback(new ObservableCollection<Invoice>(invoice), null);
        }

    }
}