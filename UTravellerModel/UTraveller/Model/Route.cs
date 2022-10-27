using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UTravellerModel.UTraveller.Model
{
    public class Route : BaseModel
    {
      
        public string Name
        {
            get;
            set;
        }

        public Color Color
        {
            get;
            set;
        }

        public ICollection<RouteCoordinates> Coordinates
        {
            get;
            set;
        }

        public ICollection<RoutePushpin> Pushpins
        {
            get;
            set;
        }


        public ICollection<RoutePolygon> Polygons
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }

    }
}
