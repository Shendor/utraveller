using System;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UTraveller.Common.Converter;

namespace UTravellerModel.UTraveller.Model
{
    public class RoutePushpin : IPushpin
    {
        private long id;
        private string description;
        private byte[] thumbnailContent;
        private GeoCoordinate coordinate;
        private BitmapImage thumbnail;
        private Color color;

        public RoutePushpin()
        {
        }


        public long Id
        {
            get { return id; }
            set { id = value; }
        }


        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        public GeoCoordinate Coordinate
        {
            get { return coordinate; }
            set { coordinate = value; }
        }


        public Color Color
        {
            get { return color; }
            set { color = value; }
        }


        public byte[] ThumbnailContent
        {
            get { return thumbnailContent; }
            set { thumbnailContent = value; }
        }


        public BitmapImage Thumbnail
        {
            get
            {
                if (thumbnail == null)
                {
                    thumbnail = new BitmapImage();
                    BytesToBitmapImageConverter.ToStream(thumbnail, thumbnailContent);
                }
                return thumbnail;
            }
        }

    }
}
