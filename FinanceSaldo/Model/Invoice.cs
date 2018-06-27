using GalaSoft.MvvmLight;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GalaSoft.MvvmLight.Messaging;

namespace FinanceSaldo.Model
{
    public class Invoice : ObservableObject
    {
        public event PropertyChangedEventHandler InvoiceSaldoChanged;
        private void PropertyChangedEvent(string property)
        {
            InvoiceSaldoChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

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
            set
            {
                Set(ref _date, value);
                RaisePropertyChanged(nameof(ExpiryDate));
                RaisePropertyChanged(nameof(IsExpiry));
            }
        }

        private decimal _debit;
        public decimal Debit
        {
            get => _debit;
            set
            {
                Set(ref _debit, value);
                PropertyChangedEvent(nameof(Debit));
            }
        }

        private decimal _credit;
        public decimal Credit
        {
            get => _credit;
            set
            {
                Set(ref _credit, value);
                PropertyChangedEvent(nameof(Credit));
            }
        }

        private int _expiryDays;
        [Range(0, 999, ErrorMessage = "Срок должен быть от 1 до 999")]
        public int ExpiryDays
        {
            get => _expiryDays;
            set
            {
                if (value >= 0 && value <= 999)
                {
                    Set(ref _expiryDays, value);
                    RaisePropertyChanged(nameof(ExpiryDate));
                    RaisePropertyChanged(nameof(IsExpiry));
                    RaisePropertyChanged(nameof(IsCash));
                }
            }
        }

        [NotMapped]
        public DateTime ExpiryDate => Date.AddDays(ExpiryDays);

        [NotMapped]
        public bool IsExpiry => ExpiryDate < DateTime.Now;

        [NotMapped]
        public bool IsCash => ExpiryDays == 0;

        public int CompanyId { get; set; }
        public virtual Company Company { get; set; }

        public Invoice()
        {
            _expiryDays = 30;
            _date = DateTime.Now;
        }
    }
}
