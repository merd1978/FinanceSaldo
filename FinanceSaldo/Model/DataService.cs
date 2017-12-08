﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using FinanceSaldo.ViewModel;

namespace FinanceSaldo.Model
{
    public class DataService : IDataService
    {
        readonly DataEntity _context;
        public DataService()
        {
            _context = new DataEntity();
            _context.Database.CreateIfNotExists();
        }
        public void GetCompany(Action<ObservableCollection<Company>, Exception> callback)
        {
            var company = _context.Company;
            callback(new ObservableCollection<Company>(company), null);
        }

        public void GetCompanyWithSaldo(Action<ObservableCollection<CompanyList>, Exception> callback)
        {
            var query = _context.Company.Select(m => new CompanyList
            {
                Company = m,
                TotalSaldo = (int)(m.Saldo + m.Invoice.Sum(o => (Decimal?)o.Debit - o.Credit) ?? 0M)
            });
            callback(new ObservableCollection<CompanyList>(query), null);
        }

        public void CreateCompany(Company company)
        {
            _context.Company.Add(company);
            _context.SaveChanges();
        }

        public void UpdateCompany(Company company)
        {
            _context.Company.Attach(company);
            _context.SaveChanges();
        }

        public void RemoveCompany(Company company)
        {
            _context.Company.Remove(company);
            _context.SaveChanges();
        }
    }
}