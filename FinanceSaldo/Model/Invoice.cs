using System;

namespace FinanceSaldo.Model
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal DebitCash { get; set; }
        public decimal CreditCash { get; set; }

        public virtual Company Company { get; set; }
    }
}
