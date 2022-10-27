using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTraveller.MoneySpendings.Message;
using UTraveller.Resources;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.MoneySpendings.ViewModel
{
    public class MoneySpendingItemViewModel : BaseTimeLineItemViewModel<MoneySpending>
    {
        public MoneySpendingItemViewModel(MoneySpending moneySpending) :base(moneySpending)
        {
        }


        public string Category
        {
            get { return MoneySpendingCategoryUtil.GetMoneySpendingCategoryName(DateItem.MoneySpendingCategory); }
        }


        public string Amount
        {
            get { return DateItem.Amount.ToString("0") + " " + CurrencyUtil.GetCurrencySymbol(DateItem.Currency); }
        }


        public string Description
        {
            get { return DateItem.Description; }
        }


        public Uri Icon
        {
            get { return MoneySpendingCategoryUtil.GetIcon(DateItem.MoneySpendingCategory); }
        }


        protected override void OnTimeLineItemDeleted()
        {
            MessengerInstance.Send<DeleteMoneySpendingMessage>(new DeleteMoneySpendingMessage(DateItem));
        }
    }
}
