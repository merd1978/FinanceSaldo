﻿using System;
using FinanceSaldo.Model;
using System.Collections.ObjectModel;
using FinanceSaldo.ViewModel;

namespace FinanceSaldo.Design
{
    public class DesignDataService : IDataService
    {
        public void GetCompany(Action<ObservableCollection<Company>, Exception> callback)
        {
            var DesignCompany = new ObservableCollection<Company>();
            for (int index = 0; index < 15; index++)
            {
                var company = new Company
                {
                    Name = "Name" + index,
                    Description = "Description" + index,
                    Saldo = 1200.55m
                };
                DesignCompany.Add(company);
            }
            callback(DesignCompany, null);
        }

        public void GetCompanyWithSaldo(Action<ObservableCollection<CompanyList>, Exception> callback)
        {
        }

        public void CreateCompany(Company company)
        {
        }

        public void UpdateCompany(Company company)
        {
        }

        //public void RemoveCompany(Company company)
        //{
        //}
        public void RemoveCompany(Company company, Action<Exception> callback) { }

        public void RemoveInvoice(Invoice invoice)
        {
        }

        public void GetInvoice4Company(Action<ObservableCollection<Invoice>, Exception> callback, Company company)
        {
            var DesignInvoice = new ObservableCollection<Invoice>();
            for (int index=0; index<15; index++)
            {
                var invoice = new Invoice
                {
                    Name = "Name" + index,
                    Date = new DateTime(2011, 1, 1),
                    Debit = 12.55m,
                    Credit = 1.40m
                };
                DesignInvoice.Add(invoice);
            }
            callback(DesignInvoice, null);
        }
    }
}