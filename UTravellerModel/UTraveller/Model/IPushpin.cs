using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public interface IPushpin
    {
        string Description
        {
            get;
            set;
        }

        GeoCoordinate Coordinate
        {
            get;
            set;
        }
    }
}
