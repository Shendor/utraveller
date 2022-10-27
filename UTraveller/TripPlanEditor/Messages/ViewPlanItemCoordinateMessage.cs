using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.TripPlanEditor.Messages
{
    public class ViewPlanItemCoordinateMessage
    {
        public ViewPlanItemCoordinateMessage(string address, GeoCoordinate coordinate)
        {
            this.Address = address;
            this.Coordinate = coordinate;
        }


        public string Address
        {
            get;
            private set;
        }


        public GeoCoordinate Coordinate
        {
            get;
            private set;
        }
    }
}
