using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UTravellerModel.UTraveller.Model
{
    public class Message : BaseModel, IPushpinItem
    {
        private static readonly Uri ICON_URI = new Uri("/Resource/Icons/message.png", UriKind.Relative);
        private static BitmapImage icon = null;

        public Message()
        {
            Date = DateTime.Now;
        }

        public string Text
        {
            get;
            set;
        }


        public DateTime Date
        {
            get;
            set;
        }


        public GeoCoordinate Coordinate
        {
            get;
            set;
        }


        public string FacebookPostId
        {
            get; 
            set;
        }


        public BitmapImage Thumbnail
        {
            get {
                if (icon == null)
                {
                    icon = new BitmapImage(ICON_URI);
                }
                return icon;
            }
        }
    }
}
