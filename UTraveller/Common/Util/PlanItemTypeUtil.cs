using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Resources;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Util
{
    public class PlanItemTypeUtil
    {
        private static readonly IDictionary<Enum, Uri> planItemTypeIcons = new Dictionary<Enum, Uri>();

        static PlanItemTypeUtil()
        {
            planItemTypeIcons.Add(PlanItemType.Cafe, new Uri("/Assets/Icons/Restaurant.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Cinema, new Uri("/Assets/Icons/cinema.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Club, new Uri("/Assets/Icons/Club.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Concert, new Uri("/Assets/Icons/Concert.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Meeting, new Uri("/Assets/Icons/meeting.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Other, new Uri("/Assets/Icons/marker.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Pub, new Uri("/Assets/Icons/Pub.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Restaurant, new Uri("/Assets/Icons/Restaurant.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Shopping, new Uri("/Assets/Icons/Shopping.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Showplace, new Uri("/Assets/Icons/showplace.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Theatre, new Uri("/Assets/Icons/theatre.png", UriKind.Relative));

            planItemTypeIcons.Add(PlanItemType.Church, new Uri("/Assets/Icons/church.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Zoo, new Uri("/Assets/Icons/zoo.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Beach, new Uri("/Assets/Icons/beach.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.AmuzementPark, new Uri("/Assets/Icons/amuzement_park.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Museum, new Uri("/Assets/Icons/museum.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Market, new Uri("/Assets/Icons/market.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.ATM, new Uri("/Assets/Icons/atm.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Bank, new Uri("/Assets/Icons/bank.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Stadium, new Uri("/Assets/Icons/stadium.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Library, new Uri("/Assets/Icons/library.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Park, new Uri("/Assets/Icons/park.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Parking, new Uri("/Assets/Icons/parking.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Entertainment, new Uri("/Assets/Icons/Entertainment.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Pool, new Uri("/Assets/Icons/pool.png", UriKind.Relative));
            planItemTypeIcons.Add(PlanItemType.Petrol, new Uri("/Assets/Icons/fuel.png", UriKind.Relative));

            planItemTypeIcons.Add(RentPlanItemType.Camping, new Uri("/Assets/Icons/tent.png", UriKind.Relative));
            planItemTypeIcons.Add(RentPlanItemType.Car, new Uri("/Assets/Icons/car_rent.png", UriKind.Relative));
            planItemTypeIcons.Add(RentPlanItemType.Motorcycle, new Uri("/Assets/Icons/motorcycle.png", UriKind.Relative));
            planItemTypeIcons.Add(RentPlanItemType.Bicycle, new Uri("/Assets/Icons/bicycle.png", UriKind.Relative));
            planItemTypeIcons.Add(RentPlanItemType.Hotel, new Uri("/Assets/Icons/Hotel.png", UriKind.Relative));
            planItemTypeIcons.Add(RentPlanItemType.House, new Uri("/Assets/Icons/House.png", UriKind.Relative));

            planItemTypeIcons.Add(TransportPlanItemType.Bus, new Uri("/Assets/Icons/Bus.png", UriKind.Relative));
            planItemTypeIcons.Add(TransportPlanItemType.Ferry, new Uri("/Assets/Icons/Ferry.png", UriKind.Relative));
            planItemTypeIcons.Add(TransportPlanItemType.Flight, new Uri("/Assets/Icons/flight.png", UriKind.Relative));
            planItemTypeIcons.Add(TransportPlanItemType.Ship, new Uri("/Assets/Icons/Ship.png", UriKind.Relative));
            planItemTypeIcons.Add(TransportPlanItemType.Taxi, new Uri("/Assets/Icons/Taxi.png", UriKind.Relative));
            planItemTypeIcons.Add(TransportPlanItemType.Train, new Uri("/Assets/Icons/Train.png", UriKind.Relative));

        }


        public static string GetPlanItemTypeName(Enum planItemType)
        {
            return AppResources.ResourceManager.GetString("PlanItemType_" + planItemType);
        }


        public static Uri GetIcon(Enum planItemType)
        {
            return planItemType != null && planItemTypeIcons.ContainsKey(planItemType) ?
                planItemTypeIcons[planItemType] : null;
        }


        public static Enum[] GetAllPlanItemTypes()
        {
            Array planItemTypes = Enum.GetValues(typeof(PlanItemType));
            Array rentItemTypes = Enum.GetValues(typeof(RentPlanItemType));
            Array transportItemTypes = Enum.GetValues(typeof(TransportPlanItemType));
            
            var combined = new Enum[planItemTypes.Length + rentItemTypes.Length + transportItemTypes.Length];
            Array.Copy(planItemTypes, combined, planItemTypes.Length);
            Array.Copy(rentItemTypes, 0, combined, planItemTypes.Length, rentItemTypes.Length);
            Array.Copy(transportItemTypes, 0, combined,
                planItemTypes.Length + rentItemTypes.Length, transportItemTypes.Length);

            return combined;
        }
    }
}
