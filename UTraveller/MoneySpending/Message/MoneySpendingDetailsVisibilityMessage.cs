using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;

namespace UTraveller.MoneySpendings.Message
{
    public class MoneySpendingDetailsVisibilityMessage : ObjectMessage<bool>
    {
        public MoneySpendingDetailsVisibilityMessage(bool isVisible)
            : base(isVisible)
        {
        }
    }
}
