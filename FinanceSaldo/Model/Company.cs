using System.Collections.Generic;

namespace FinanceSaldo.Model
{
    public class Company
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Saldo { get; set; }
        public virtual ICollection<Invoice> Invoice { get; set; }

        public Company()
        {
            Invoice = new HashSet<Invoice>();
        }
    }
}
