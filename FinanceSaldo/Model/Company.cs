using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FinanceSaldo.Model
{
    public class Company
    {
        public int CompanyId { get; set; }
        [Required(AllowEmptyStrings  = false, ErrorMessage = "Название не может быть пустым")]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Saldo { get; set; }
        public virtual ICollection<Invoice> Invoice { get; set; }

        //[NotMapped]
        //public decimal Total
        //{
        //    get { Invoice.Sum()}
        //}

        public Company()
        {
            Invoice = new HashSet<Invoice>();
        }
    }
}
