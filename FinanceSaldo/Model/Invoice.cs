using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceSaldo.Model
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal DebitCash { get; set; }
        public decimal CreditCash { get; set; }

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }
    }
}
