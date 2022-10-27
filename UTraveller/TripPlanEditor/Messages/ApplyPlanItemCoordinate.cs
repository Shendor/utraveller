using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.TripPlanEditor.Messages
{
    public class ApplyPlanItemCoordinate
    {
        public ApplyPlanItemCoordinate(GeoCoordinate coordinate)
        {
            Coordinate = coordinate;
        }


        public GeoCoordinate Coordinate
        {
            get;
            set;
        }

    }
}
