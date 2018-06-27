using System;
using System.Collections.Generic;
using FinanceSaldo.Model;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FinanceSaldo.ViewModel;

namespace FinanceSaldo.Design
{
    public class DesignDataService : IDataService
    {
        public void GetCompany(Action<ObservableCollection<Company>, Exception> callback)
        {
            var designCompany = new ObservableCollection<Company>();
            for (int index = 0; index < 15; index++)
            {
                var company = new Company
                {
                    Name = "Name" + index,
                    Description = "Description" + index,
                    Saldo = 1200.55m
                };
                designCompany.Add(company);
            }
            callback(designCompany, null);
        }

        public async Task<List<Company>> GetCompanyAsync()
        {
            var designCompany = new List<Company>();
            for (int index = 0; index < 15; index++)
            {
                var company = new Company
                {
                    Name = "Name" + index,
                    Description = "Description" + index,
                    Saldo = 1200.55m
                };
                designCompany.Add(company);
            }
            return designCompany;
        }


        public void InsertOrUpdateCompany(Company company)
        {
        }

        public void SaveChanges()
        {
        }

        public bool HasUnsavedChanges()
        {
            return false;
        }

        public void RemoveCompany(Company company, Action<Exception> callback) { }

        public void RemoveInvoice(Invoice invoice)
        {
        }

        public void GetInvoice4Company(Action<ObservableCollection<Invoice>, Exception> callback, Company company)
        {
            var designInvoice = new ObservableCollection<Invoice>();
            for (int index=0; index<15; index++)
            {
                var invoice = new Invoice
                {
                    Name = "Name" + index,
                    Date = new DateTime(2011, 1, 1),
                    Debit = 12.55m,
                    Credit = 1.40m
                };
                designInvoice.Add(invoice);
            }
            callback(designInvoice, null);
        }
    }
}