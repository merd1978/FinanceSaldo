using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FinanceSaldo.ViewModel;
using GalaSoft.MvvmLight;

namespace FinanceSaldo.Model
{
    public class Company : ObservableObject
    {
        public int CompanyId { get; set; }
        [Required(AllowEmptyStrings  = false, ErrorMessage = "Название не может быть пустым")]
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Saldo { get; set; }

        public virtual ObservableCollection<Invoice> Invoice { get; set; }

        [NotMapped]
        public int TotalSaldo
        {
            get { return (int)(Saldo + Invoice.Sum(o => (decimal?)o.Debit - o.Credit) ?? 0M); }
        }

        [NotMapped]
        public InvoiceViewModel InvoiceViewModel { get; set; }
    }
}
