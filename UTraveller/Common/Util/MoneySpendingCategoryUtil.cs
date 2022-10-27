using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Resources;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Util
{
    public class MoneySpendingCategoryUtil
    {
        private static readonly IDictionary<MoneySpendingCategory, Uri> moneySpendingIcons =
            new Dictionary<MoneySpendingCategory, Uri>();

        static MoneySpendingCategoryUtil()
        {

            moneySpendingIcons.Add(MoneySpendingCategory.Hotel, new Uri("/Assets/Icons/Hotel.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Club, new Uri("/Assets/Icons/Club.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Concert, new Uri("/Assets/Icons/Concert.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Entertainment, new Uri("/Assets/Icons/Entertainment.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.FastFood, new Uri("/Assets/Icons/fast_food.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Fine, new Uri("/Assets/Icons/Fine.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.House, new Uri("/Assets/Icons/House.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Parking, new Uri("/Assets/Icons/parking.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Pub, new Uri("/Assets/Icons/Pub.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.PublicTransport, new Uri("/Assets/Icons/public_transport.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Restaurant, new Uri("/Assets/Icons/Restaurant.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Shopping, new Uri("/Assets/Icons/Shopping.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Souvenir, new Uri("/Assets/Icons/gift.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.TransportRent, new Uri("/Assets/Icons/car_rent.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Camping, new Uri("/Assets/Icons/tent.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Museum, new Uri("/Assets/Icons/museum.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Showplace, new Uri("/Assets/Icons/showplace.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Grocery, new Uri("/Assets/Icons/cart.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Internet, new Uri("/Assets/Icons/internet.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Phone, new Uri("/Assets/Icons/Phone.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Fuel, new Uri("/Assets/Icons/fuel.png", UriKind.Relative));
            moneySpendingIcons.Add(MoneySpendingCategory.Other, new Uri("/Assets/Icons/other.png", UriKind.Relative));
        }


        public static string GetMoneySpendingCategoryName(MoneySpendingCategory category)
        {
            return AppResources.ResourceManager.GetString("Spendings_Category_" + category);
        }


        public static Uri GetIcon(MoneySpendingCategory moneySpendingType)
        {
            return moneySpendingIcons.ContainsKey(moneySpendingType) ? moneySpendingIcons[moneySpendingType] : null;
        }
    }
}
