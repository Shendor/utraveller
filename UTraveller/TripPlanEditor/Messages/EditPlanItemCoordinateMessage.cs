using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.TripPlanEditor.Messages
{
    public class EditPlanItemCoordinateMessage
    {
        public EditPlanItemCoordinateMessage(GeoCoordinate coordinate)
        {
            this.Coordinate = coordinate;
        }


        public GeoCoordinate Coordinate
        {
            get;
            private set;
        }
    }
}
