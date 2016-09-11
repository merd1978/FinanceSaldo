using System;
using FinanceSaldo.Model;
using System.Collections.ObjectModel;

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
        public void GetInvoice(Action<ObservableCollection<Invoice>, Exception> callback)
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