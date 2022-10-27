using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UTravellerModel.UTraveller.Model
{
    public interface IPushpinItem : IDateItem
    {
        GeoCoordinate Coordinate
        {
            get;
            set;
        }


        BitmapImage Thumbnail
        {
            get;
        }
    }
}
