using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IExpenseService
    {
        void AddExpense(MoneySpending moneySpending, Event e);

        void DeleteExpense(MoneySpending moneySpending, Event e);

        IEnumerable<MoneySpending> GetExpenses(Event e);

        IDictionary<CurrencyType, decimal> GetTotalSpentMoneyForEvent(Event e);
    }
}
