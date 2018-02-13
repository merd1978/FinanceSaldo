using GalaSoft.MvvmLight;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceSaldo.Model
{
    public class Invoice : ObservableObject
    {
        public int InvoiceId { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private DateTime _date;
        [Column(TypeName = "datetime2")]
        public DateTime Date
        {
            get => _date;
            set => Set(ref _date, value);
        }

        private decimal _debit;
        public decimal Debit
        {
            get => _debit;
            set => Set(ref _debit, value);
        }

        private decimal _credit;
        public decimal Credit
        {
            get => _credit;
            set => Set(ref _credit, value);
        }

        private int _expiryDays;
        public int ExpiryDays
        {
            get => _expiryDays;
            set => Set(ref _expiryDays, value);
        }

        private decimal _debitCash;
        public decimal DebitCash
        {
            get => _debitCash;
            set => Set(ref _debitCash, value);
        }

        private decimal _creditCash;
        public decimal CreditCash
        {
            get => _creditCash;
            set => Set(ref _creditCash, value);
        }

        [NotMapped] public DateTime ExpiryDate => Date.AddDays(ExpiryDays);

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public Invoice()
        {
            _expiryDays = 40;
            _date = DateTime.Now;
        }
    }
}
