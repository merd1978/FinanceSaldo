using GalaSoft.MvvmLight;

namespace FinanceSaldo.ViewModel
{
    public class HelpViewModel : ViewModelBase
    {
        public string TabName => "Помощь";

        public string Title => "Finance Saldo";

        public string About => "Приложение Financesaldo разработано для проведения расчётов между несколькими субъектами хозяйствования." +
                               " Вы можете сохранить, редактировать, удалить историю операции, построить отчёт о проведении операции." +
                               " Нажав на кнопку поиск, вы сможете найти нужную вам операцию. Нажав на кнопку печать, вы распечатаете отчёт" +
                               " о истории взаиморасчётов нужной вам организации. ";

        public string Bottom => "Год разработки 2017. Разработано в учебных целях.";
    }
}
