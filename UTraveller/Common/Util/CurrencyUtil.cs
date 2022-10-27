using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Resources;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Util
{
    public class CurrencyUtil
    {
        private static readonly IDictionary<CurrencyType, string> currencies =
            new Dictionary<CurrencyType, string>();

        static CurrencyUtil()
        {

            currencies.Add(CurrencyType.Euro, char.ConvertFromUtf32(0x20ac));
            currencies.Add(CurrencyType.HUForint, "HUF");
            currencies.Add(CurrencyType.ROLei,"RON");
            currencies.Add(CurrencyType.RURuble,"RUB");
            currencies.Add(CurrencyType.UAHryvnia, "UAH");
            currencies.Add(CurrencyType.UKPound, char.ConvertFromUtf32(0x00A3));
            currencies.Add(CurrencyType.USDollar, "$");
            currencies.Add(CurrencyType.AUDollar, "AUD");
            currencies.Add(CurrencyType.CADollar, "CAD");
            currencies.Add(CurrencyType.BULev, "BGN");
            currencies.Add(CurrencyType.CHFrank, "CHF");
            currencies.Add(CurrencyType.CZKoruna, "CZK");
            currencies.Add(CurrencyType.DNKrona, "DKK");
            currencies.Add(CurrencyType.ICKrona, "ISK");
            currencies.Add(CurrencyType.SWKrona, "SEK");
            currencies.Add(CurrencyType.TRLira, "TRY");
            currencies.Add(CurrencyType.NRKrona, "NOK");
            currencies.Add(CurrencyType.PLZloty, "PLN");

            currencies.Add(CurrencyType.UAEDirham, "AED");
            currencies.Add(CurrencyType.MDLei, "MDL");
            currencies.Add(CurrencyType.MKDenar, "MKD");
            currencies.Add(CurrencyType.KOWon, "KRW");
            currencies.Add(CurrencyType.JPYen, "JPY");
            currencies.Add(CurrencyType.ISShekel, "ILS");
            currencies.Add(CurrencyType.INRupee, "INR");
            currencies.Add(CurrencyType.CNYuan, "CNY");
            currencies.Add(CurrencyType.AZManat, "AZM");
            currencies.Add(CurrencyType.ARDram, "AMD");
        }


        public static string GetCurrencyName(CurrencyType currencyType)
        {
            return AppResources.ResourceManager.GetString("Spendings_Currency_" + currencyType);
        }

        public static string GetCurrencySymbol(CurrencyType currencyType)
        {
            return currencies[currencyType];
        }
    }
}
