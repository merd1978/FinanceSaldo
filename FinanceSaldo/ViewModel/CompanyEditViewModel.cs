using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FinanceSaldo.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FinanceSaldo.ViewModel
{
    public class CompanyEditViewModel : ViewModelBase, IDataErrorInfo
    {
        private readonly IDataService _dataService;

        public string TabName
        {
            get;
            private set;
        }

        private Company _company = new Company();
        public Company Company
        {
            get => _company;
            set => Set(ref _company, value);
        }


        [Required(AllowEmptyStrings = false, ErrorMessage = "Поле не может быть пустым")]
        public string CompanyName
        {
            get => Company.Name;
            set { Company.Name = value; RaisePropertyChanged(() => CompanyName); }
        }

        //enable or disable save button
        private bool _canSave;
        public bool CanSave
        {
            get => _canSave;
            set => Set(ref _canSave, value);
        }

        public RelayCommand SaveCommand { get; set; }
        private void ExecuteSaveCommand()
        {
            _dataService.CreateCompany(_company);
            Messenger.Default.Send(new NotificationMessage("RefreshCompany"));
        }

        public RelayCommand CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand()
        {
            Messenger.Default.Send(new NotificationMessage("CloseCurrentTab"));
        }

        public CompanyEditViewModel(string tabName, IDataService dataService)
        {
            TabName = tabName;
            _dataService = dataService;

            SaveCommand = new RelayCommand(ExecuteSaveCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                //string result = string.Empty;
                //switch (columnName)
                //{
                //    case "Name": if (string.IsNullOrEmpty(Company.Name)) result = "Name is required!"; break;
                //};
                //return result;

                var value = GetType().GetProperty(columnName)?.GetValue(this, null);
                var results = new List<ValidationResult>();

                var context = new ValidationContext(this, null, null) { MemberName = columnName };

                if (!Validator.TryValidateProperty(value, context, results))
                {
                    CanSave = false;
                    return results.First().ErrorMessage;
                }

                CanSave = true;
                return string.Empty;
            }
        }

        public string Error => throw new NotImplementedException();
    }
}