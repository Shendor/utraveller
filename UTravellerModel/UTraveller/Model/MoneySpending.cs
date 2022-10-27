using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public class MoneySpending : BaseModel, IDateItem
    {
        private MoneySpendingCategory moneySpendingCategory;
        private decimal amount;
        private string description;
        private CurrencyType currency;
        private DateTime date;


        public MoneySpendingCategory MoneySpendingCategory
        {
            get { return moneySpendingCategory; }
            set { moneySpendingCategory = value; }
        }


        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }


        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        public CurrencyType Currency
        {
            get { return currency; }
            set { currency = value; }
        }


        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }
    }
}
