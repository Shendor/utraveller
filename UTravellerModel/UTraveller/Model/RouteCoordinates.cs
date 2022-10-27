using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public class RouteCoordinates
    {
        public RouteCoordinates(int routeNumber)
        {
            RouteNumber = routeNumber;
            Coordinates = new List<GeoLocationModel>();
        }


        public IList<GeoLocationModel> Coordinates
        {
            get;
            set;
        }


        public int RouteNumber
        {
            get;
            set;
        }
    }
}
