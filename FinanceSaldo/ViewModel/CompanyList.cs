using System;
using System.Linq;
using FinanceSaldo.Model;

namespace FinanceSaldo.ViewModel
{
    public class CompanyList
    {
        public Company Company { get; set; }

        public int TotalSaldo
        {
            get { return (int) (Company.Saldo + Company.Invoice.Sum(o => (Decimal?) o.Debit - o.Credit) ?? 0M); }
        }
        public InvoiceViewModel InvoiceViewModel { get; set; }
    }
}