using GalaSoft.MvvmLight;
using FinanceSaldo.Model;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;

namespace FinanceSaldo.ViewModel
{
    /// This class contains properties that the main View can data bind to.
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }
            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        public ViewModelBase CurrentView { get; set; }      //manage current view
        public RelayCommand CompanyEditCommand { get; set; }

        private void ExecuteCompanyEditCommand()
        {

        }

        ObservableCollection<Company> _Company;
        public ObservableCollection<Company> Company
        {
            get { return _Company; }
            set
            {
                Set(ref _Company, value);
            }
        }
                        
        ObservableCollection<Invoice> _Invoice;
        public ObservableCollection<Invoice> Invoice
        {
            get { return _Invoice; }
            set
            {
                Set(ref _Invoice, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Company = new ObservableCollection<Company>();
            _dataService.GetCompany(
                (items, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    Company = items;
                });
            Invoice = new ObservableCollection<Invoice>();
            _dataService.GetInvoice(
                (items, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    Invoice = items;
                });

            CompanyEditCommand = new RelayCommand(ExecuteCompanyEditCommand);
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}