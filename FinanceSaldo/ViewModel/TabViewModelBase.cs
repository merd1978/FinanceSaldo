using GalaSoft.MvvmLight;

namespace FinanceSaldo.ViewModel
{
    public abstract class TabViewModelBase : ViewModelBase
    {
        public string TabName
        {
            get;
            private set;
        }

        protected TabViewModelBase(string tabName)
        {
            TabName = tabName;
        }
    }
}