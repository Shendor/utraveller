using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTraveller.MoneySpendings.ViewModel;

namespace UTraveller.MoneySpendings.Control
{
    public class MoneySpendingDetailsViewModel : BaseViewModel
    {
        private MoneySpendingItemViewModel moneySpendingItem;

        public override void Cleanup()
        {
            MoneySpendingItem = null;
        }


        public MoneySpendingItemViewModel MoneySpendingItem
        {
            get { return moneySpendingItem; }
            set
            {
                moneySpendingItem = value;
                RaisePropertyChanged("MoneySpendingItem");
            }
        }
    }
}
