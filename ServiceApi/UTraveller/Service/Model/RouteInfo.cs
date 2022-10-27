using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace ServiceApi.UTraveller.Service.Model
{
    public class RouteInfo
    {
        public RouteInfo()
        {
            Pushpins = new List<RoutePushpin>();
            Polygons = new List<RoutePolygon>();
        }

        public string Name
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }


        public IList<RouteCoordinates> Coordinates
        {
            get;
            set;
        }

        public IList<RoutePushpin> Pushpins
        {
            get;
            set;
        }


        public IList<RoutePolygon> Polygons
        {
            get;
            set;
        }

    }
}
