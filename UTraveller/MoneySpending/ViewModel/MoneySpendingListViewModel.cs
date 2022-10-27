using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.MoneySpendings.ViewModel
{
    public class MoneySpendingListViewModel : BaseListViewModel<MoneySpendingItemViewModel>
    {
        public MoneySpendingListViewModel(IGrouping<string, MoneySpendingItemViewModel> moneySpendingGroup)
            : base(moneySpendingGroup)
        {
            Day = moneySpendingGroup.Key;
            Total = new ObservableCollection<string>();
            var groupedTotals = from item in moneySpendingGroup
                                group item.DateItem by item.DateItem.Currency into groupedItem
                                select groupedItem;
            foreach (var total in groupedTotals)
            {
                decimal amount = 0;
                foreach (var item in total)
                {
                    amount += item.Amount;
                }
                Total.Add(amount.ToString("0") + " " + CurrencyUtil.GetCurrencySymbol(total.Key));
            }
        }


        public ObservableCollection<string> Total
        {
            get;
            private set;
        }


        public string Day
        {
            protected set;
            get;
        }


        public override bool Equals(object obj)
        {
            if (obj is MoneySpendingListViewModel)
            {
                return Day.Equals(((MoneySpendingListViewModel)obj).Day);
            }
            return false;
        }


        public override int GetHashCode()
        {
            return Day.GetHashCode();
        }


        public override string ToString()
        {
            return Day;
        }
    }
}
