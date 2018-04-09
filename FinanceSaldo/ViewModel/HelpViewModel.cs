namespace FinanceSaldo.ViewModel
{
    public class HelpViewModel : TabViewModelBase
    {
        public string Title => "Сальдо ОАО БЗСП";

        public string About => "Приложение FinanceSaldo разработано для проведения расчётов между несколькими субъектами хозяйствования.";
    

        public string Bottom => "Дата разработки 2018г. Разработано отделом ОАСУП \"ОАО БЗСП.\"";

        public HelpViewModel() : base("Помощь")
        {
        }
    }
}
