using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Model.Remote
{
    public class RouteRemoteModel : BaseRemoteModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

        public string PushpinsJson
        {
            get;
            set;
        }

        public string CoordinatesJson
        {
            get;
            set;
        }

        public string PolygonsJson
        {
            get;
            set;
        }
    }
}
