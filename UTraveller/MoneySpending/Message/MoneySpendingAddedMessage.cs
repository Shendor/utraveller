using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.MoneySpendings.Message
{
    public class MoneySpendingAddedMessage : ObjectMessage<MoneySpending>
    {
        public MoneySpendingAddedMessage(MoneySpending moneySpending)
            : base(moneySpending)
        {
        }
    }
}
