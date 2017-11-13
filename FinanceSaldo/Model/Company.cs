using System.Collections.Generic;

namespace FinanceSaldo.Model
{
    public sealed class Company
    {
        public Company()
        {
            Invoice = new HashSet<Invoice>();
        }

        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Saldo { get; set; }
        public ICollection<Invoice> Invoice { get; set; }
    }
}
