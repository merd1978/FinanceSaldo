using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FinanceSaldo.Model;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace FinanceSaldo.ViewModel
{
    public class CompanyEditViewModel : TabViewModelBase, IDataErrorInfo
    {
        private readonly IDataService _dataService;

        private Company _company;
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

        public RelayCommand SaveCommand { get; set; }
        private void ExecuteSaveCommand()
        {
            _dataService.CreateCompany(Company);
            Messenger.Default.Send(Company);
            Messenger.Default.Send(new NotificationMessage("CloseCurrentTab"));
        }

        private bool _canSave;
        private bool CanSave
        {
            get => _canSave;
            set
            {
                _canSave = value;
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private bool CanExecuteSaveCommand()
        {
            return CanSave;
        }

        public RelayCommand CloseTabCommand { get; set; }
        private void ExecuteCloseTabCommand()
        {
            Messenger.Default.Send(new NotificationMessage("CloseCurrentTab"));
        }

        public CompanyEditViewModel(IDataService dataService, Company company) : base(company.Name)
        {
            _dataService = dataService;
            Company = company;

            SaveCommand = new RelayCommand(ExecuteSaveCommand, CanExecuteSaveCommand);
            CloseTabCommand = new RelayCommand(ExecuteCloseTabCommand);
        }

        public string this[string columnName]
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