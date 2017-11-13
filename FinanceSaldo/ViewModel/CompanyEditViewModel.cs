using GalaSoft.MvvmLight;

namespace FinanceSaldo.ViewModel
{
    public class CompanyEditViewModel : ViewModelBase
    {
        public string TabName
        {
            get;
            private set;
        }

        public CompanyEditViewModel(string tabName)
        {
            TabName = tabName;
        }

    }
}