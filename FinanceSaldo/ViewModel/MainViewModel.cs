﻿using GalaSoft.MvvmLight;
using FinanceSaldo.Model;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;

namespace FinanceSaldo.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// </summary>
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
            get => _welcomeTitle;
            set => Set(ref _welcomeTitle, value);
        }

        #region Commands
        public RelayCommand CompanyEditCommand { get; set; }
        private void ExecuteCompanyEditCommand()
        {
            //CurrentView = View.CompanyView;
        }

        public RelayCommand CompanyNewCommand { get; set; }
        private void ExecuteCompanyNewCommand()
        {
            TabCollection.Add(new CompanyEditViewModel("New"));
            SelectedTabIndex = TabCollection.Count - 1;
        }

        public RelayCommand<int> CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand(int index)
        {
            TabCollection.RemoveAt(index);
        }
        #endregion

        ObservableCollection<Company> _company;
        public ObservableCollection<Company> Company
        {
            get => _company;
            set => Set(ref _company, value);
        }

        public ObservableCollection<ViewModelBase> TabCollection { get; } = new ObservableCollection<ViewModelBase>();
        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => Set(ref _selectedTabIndex, value);
        }

        ObservableCollection<Invoice> _invoice;
        public ObservableCollection<Invoice> Invoice
        {
            get => _invoice;
            set => Set(ref _invoice, value);
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
            CompanyNewCommand = new RelayCommand(ExecuteCompanyNewCommand);
            CloseTabCommand = new RelayCommand<int>(ExecuteCloseTabCommand);

            TabCollection.Add(new InvoiceViewModel("Tab1"));
            TabCollection.Add(new InvoiceViewModel("Tab2"));
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}