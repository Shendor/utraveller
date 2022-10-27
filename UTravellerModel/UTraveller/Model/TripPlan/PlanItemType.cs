using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public enum PlanItemType
    {
        Other = 0,
        Concert = 1,
        Meeting = 2,
        Showplace = 3,
        Restaurant = 4,
        Cafe = 5,
        Pub = 6,
        Club = 7,
        Theatre = 8,
        Cinema = 9,
        Shopping = 10,

        Church = 11,
        Zoo = 12,
        Beach = 13,
        AmuzementPark = 14,
        Museum = 15,
        Market = 16,
        ATM = 17,
        Bank = 18,
        Stadium = 19,
        Library = 20,
        Park = 21,
        Parking = 22,
        Entertainment = 23,
        Pool = 24,
        Petrol = 25
    }


    public enum RentPlanItemType
    {
        Car = 0,
        Hotel = 1,
        House = 2,
        Camping = 3,
        Motorcycle = 4,
        Bicycle = 5
    }

    public enum TransportPlanItemType
    {
        Taxi = 0,
        Bus = 1,
        Ferry = 2,
        Ship = 3,
        Flight = 4,
        Train = 5
    }
}
