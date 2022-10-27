using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Device.Location;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using UTraveller.Common.Converter;

namespace UTravellerModel.UTraveller.Model
{
    public class Photo : BaseModel, IDisposable, IPushpinItem, IComparable<Photo>
    {
        public static readonly int DEFAULT_COMPRESS_WIDTH = 1024;
        public static readonly int DEFAULT_COMPRESS_HEIGHT = 768;

        private string name;
        private DateTime date;
        private BitmapImage image;
        private BitmapImage thumbnail;
        private string imageUrl;
        private byte[] thumbnailContent;
        private Stream imageStream;
        private string description;
        private GeoCoordinate coordinate;
        private string facebookPostId;
        private string facebookPhotoId;

        public Photo()
        {
        }

        public Photo(Picture picture)
        {
            name = picture.Name;
            date = picture.Date;
            thumbnailContent = BytesToBitmapImageConverter.ToBytes(picture.GetThumbnail());
            Width = picture.Width;
            Height = picture.Height;
        }

        public Photo(Photo photo)
        {
            name = photo.Name;
            date = photo.Date;
            thumbnailContent = photo.thumbnailContent;
            imageStream = photo.imageStream;
            Width = photo.Width;
            Height = photo.Height;
        }


        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }


        public string Description
        {
            get { return description; }
            set { description = value; }
        }


        public int Width
        {
            get;
            set;
        }


        public int Height
        {
            get;
            set;
        }


        public GeoCoordinate Coordinate
        {
            get { return coordinate; }
            set { coordinate = value; }
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


        public BitmapImage Image
        {
            get
            {
                if (image == null)
                {
                    image = new BitmapImage();
                };
                return image;
            }
            set { image = value; }
        }


        public Stream ImageStream
        {
            get { return imageStream; }
            set { imageStream = value; }
        }


        public string FacebookPhotoId
        {
            get { return facebookPhotoId; }
            set { facebookPhotoId = value; }
        }


        public string FacebookPostId
        {
            get { return facebookPostId; }
            set { facebookPostId = value; }
        }


        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }


        public void InitializeImage()
        {
            if (imageStream != null)
            {
                Image.SetSource(imageStream);
            }
        }

        public void InitializeImage(Stream imageStream)
        {
            if (imageStream != null && this.imageStream == null)
            {
                this.imageStream = imageStream;
                Image.DecodePixelWidth = Width == 0 || (Width > DEFAULT_COMPRESS_WIDTH && Width >= Height) ? DEFAULT_COMPRESS_WIDTH : 0;
                Image.DecodePixelHeight = Image.DecodePixelWidth == 0 && Height > DEFAULT_COMPRESS_WIDTH && Width < Height ? DEFAULT_COMPRESS_WIDTH : 0;
                Image.SetSource(imageStream);
            }
            else if (ImageUrl != null && imageStream == null)
            {
                Image.UriSource = new Uri(ImageUrl);
            }
        }


        public void Dispose()
        {
            if (imageStream != null)
            {
                imageStream.Dispose();
                imageStream = null;
            }
            if (image != null)
            {
                image.ClearValue(System.Windows.Media.Imaging.BitmapImage.UriSourceProperty);
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            if (obj == null || !obj.GetType().Equals(this.GetType()))
            {
                return false;
            }
            var photoItem = (Photo)obj;
            return photoItem.Name.Equals(Name) && photoItem.Date.Equals(Date);
        }


        public override int GetHashCode()
        {
            int hashCode = 37 * Name.GetHashCode();
            hashCode += 37 * Date.GetHashCode();

            return hashCode;
        }


        public int CompareTo(Photo other)
        {
            return (Date != null && other.Date != null) ? -Date.CompareTo(other.Date) : 0;
        }
    }
}
