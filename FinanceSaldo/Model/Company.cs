using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using FinanceSaldo.ViewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;

namespace FinanceSaldo.Model
{
    public class Company : ObservableObject
    {
        public int CompanyId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Название не может быть пустым")]
        private string _name = "Новая";
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
        public string Description { get; set; }
        private decimal _saldo;
        public decimal Saldo
        {
            get => _saldo;
            set
            {
                Set(ref _saldo, value);
                RaisePropertyChanged(nameof(TotalSaldo));
                Messenger.Default.Send(new NotificationMessage("CurrentSaldoChanged"));
            }
        }

        public virtual ObservableCollection<Invoice> Invoice { get; private set; }

        [NotMapped]
        public decimal TotalSaldo
        {
            get { return (Saldo + Invoice.Sum(o => (decimal?)o.Debit - o.Credit) ?? 0M); }
        }

        [NotMapped]
        public InvoiceViewModel InvoiceViewModel { get; set; }

        public decimal GetSaldo(DateTime date)
        {
            return (Saldo + Invoice.Where(d => d.Date.Date <= date.Date).Sum(o => (decimal?) o.Debit - o.Credit) ?? 0M);
        }

        public Company()
        {
            Invoice = new ObservableCollection<Invoice>();
        }
    }
}
