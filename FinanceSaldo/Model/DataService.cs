using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace FinanceSaldo.Model
{
    public class DataService : IDataService
    {
        readonly DataEntity _context;
        public DataService()
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            _context = new DataEntity();
            _context.Database.CreateIfNotExists();
        }
        public void GetCompany(Action<ObservableCollection<Company>, Exception> callback)
        {
            var query = _context.Company;
            callback(new ObservableCollection<Company>(query), null);
        }

        public void InsertOrUpdateCompany(Company company)
        {
            _context.Entry(company).State = company.CompanyId == 0 ? EntityState.Added : EntityState.Modified;
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public bool HasUnsavedChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public void RemoveCompany(Company company, Action<Exception> callback)
        {
            try
            {
                _context.Company.Remove(company);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                callback(ex);
            }
        }

        public void RemoveInvoice(Invoice invoice)
        {
            if (invoice == null) return;
            if (_context.Invoice.Any(o => o.InvoiceId == invoice.InvoiceId))
            {
                _context.Invoice.Remove(invoice);
                _context.SaveChanges();
            }
        }
    }
}