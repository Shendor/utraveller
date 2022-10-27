using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.MoneySpendings.Message
{
    public class DeleteMoneySpendingMessage : ObjectMessage<MoneySpending>
    {
        public DeleteMoneySpendingMessage(MoneySpending moneySpending)
            : base(moneySpending)
        {
        }
    }
}
