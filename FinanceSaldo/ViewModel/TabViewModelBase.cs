using GalaSoft.MvvmLight;

namespace FinanceSaldo.ViewModel
{
    public abstract class TabViewModelBase : ViewModelBase
    {
        private string _tabName;
        public string TabName
        {
            get => _tabName;
            set => Set(ref _tabName, value);
        }

        protected TabViewModelBase(string tabName)
        {
            TabName = tabName;
        }
    }
}