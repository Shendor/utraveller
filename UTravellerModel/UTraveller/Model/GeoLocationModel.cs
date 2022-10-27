using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTravellerModel.UTraveller.Model
{
    public class GeoLocationModel
    {
        public GeoLocationModel()
        {
        }


        public GeoLocationModel(double latitude, double longitude)
        {
            Lat = (float)latitude;
            Lng = (float)longitude;
        }


        public float Lat
        {
            get;
            set;
        }


        public float Lng
        {
            get;
            set;
        }
    }
}
