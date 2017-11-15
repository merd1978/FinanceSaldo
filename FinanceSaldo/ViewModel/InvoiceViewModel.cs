using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FinanceSaldo.ViewModel
{
    public class InvoiceViewModel : ViewModelBase
    {
        public string TabName
        {
            get;
            private set;
        }

        public RelayCommand<object> CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand(object index)
        {
            //TabCollection.RemoveAt((int)index);
        }

        public InvoiceViewModel(string tabName)
        {
            TabName = tabName;
            CloseTabCommand = new RelayCommand<object>(ExecuteCloseTabCommand);
        }
    }
}