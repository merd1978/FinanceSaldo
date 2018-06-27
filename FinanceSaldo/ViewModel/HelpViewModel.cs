namespace FinanceSaldo.ViewModel
{
    public class HelpViewModel : TabViewModelBase
    {
        public string Title => "Сальдо ОАО БЗСП v1.0";

        public string About => $"Приложение FinanceSaldo разработано для проведения расчётов между несколькими субъектами хозяйствования." +
                            "\n\nРасчетные формулы:" +
                            "\nПросроченное сальдо = Начальное сальдо + Дебит - Кредит (с истекшим сроком погашения)" +
                            "\nСальдо = Просроченное сальдо + Дебит - Кредит (срок погашения НЕ истек)";
    
        public string Bottom => "Дата разработки 2018г. Разработано отделом ОАСУП \"ОАО БЗСП.\"";

        public HelpViewModel() : base("Помощь")
        { 
        }
    }
}
