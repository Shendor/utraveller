using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UTravellerModel.UTraveller.Model
{
    public class RoutePolygon
    {
        public RoutePolygon()
        {
            Coordinates = new List<double>();
        }


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


        public float Opacity
        {
            get;
            set;
        }


        public IList<double> Coordinates
        {
            get;
            set;
        }
    }
}
