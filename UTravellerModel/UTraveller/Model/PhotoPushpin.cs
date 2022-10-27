using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace UTravellerModel.UTraveller.Model
{
    public class PhotoPushpin
    {
        private long id;
        private string description;
        private BitmapImage thumbnail;
        private GeoCoordinate coordinate;
        private ICollection<Photo> photos;

        public PhotoPushpin()
        {
            photos = new HashSet<Photo>();
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

        public ICollection<Photo> Photos
        {
            get { return photos; }
        }

        public BitmapImage Thumbnail
        {
            get
            {
                var photo = photos.FirstOrDefault();
                if (photo != null)
                {
                    thumbnail = photo.Thumbnail;
                }
                return thumbnail;
            }
            set { thumbnail = value; }
        }

        public void AddPhoto(Photo photo)
        {
            photos.Add(photo);
        }
    }
}
