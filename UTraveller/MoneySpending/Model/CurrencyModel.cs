using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.MoneySpendings.Model
{
    public class CurrencyModel : IComparable<CurrencyModel>
    {
        public CurrencyModel(CurrencyType currency, string name, string symbol)
        {
            Currency = currency;
            Name = name;
            Symbol = symbol;
        }


        public CurrencyType Currency
        {
            get;
            private set;
        }


        public string Name
        {
            get;
            private set;
        }


        public string Symbol
        {
            get;
            private set;
        }


        public override string ToString()
        {
            return Symbol;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CurrencyModel))
            {
                return false;
            }

            return ((CurrencyModel)obj).Currency == this.Currency;
        }

        public override int GetHashCode()
        {
            return this.Currency.GetHashCode();
        }

        public int CompareTo(CurrencyModel other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
