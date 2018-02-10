using GalaSoft.MvvmLight;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceSaldo.Model
{
    public class Invoice : ObservableObject
    {
        public int InvoiceId { get; set; }
        public string Name { get; set; }
        private DateTime _date;
        [Column(TypeName = "datetime2")]
        public DateTime Date
        {
            get => _date;
            set
            {
                Set(ref _date, value);
                RaisePropertyChanged("ExpiryDate");
            }
        }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        private int _expiryDays;
        public int ExpiryDays
        {
            get => _expiryDays;
            set
            {
                Set(ref _expiryDays, value);
                RaisePropertyChanged("ExpiryDate");
            }
        }

        public decimal DebitCash { get; set; }
        public decimal CreditCash { get; set; }

        [NotMapped]
        public DateTime ExpiryDate
        {
            get { return Date.AddDays(ExpiryDays); }
        }

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public Invoice()
        {
            _expiryDays = 40;
            _date = DateTime.Now;
        }
    }
}
