using GalaSoft.MvvmLight;

namespace FinanceSaldo.ViewModel
{
    public class InvoiceViewModel : ViewModelBase
    {
        public string TabName
        {
            get;
            private set;
        }

        public InvoiceViewModel(string tabName)
        {
            TabName = tabName;
        }
    }
}