using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api.Remote
{
    public interface IMoneySpendingRemoteService
    {
        Task<bool> AddMoneySpendingForEvent(MoneySpending moneySpending, Event e);

        Task<bool> DeleteMoneySpending(MoneySpending moneySpending, Event e);

        Task<IEnumerable<MoneySpending>> GetMoneySpendingsForEvent(Event e);
    }
}
