using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public class BasePlanItem : IPushpin
    {

        public DateTime? Date
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }


        public string Address
        {
            get;
            set;
        }


        public GeoCoordinate Coordinate
        {
            get;
            set;
        }


        public bool IsVisited
        {
            get;
            set;
        }

    }
}
