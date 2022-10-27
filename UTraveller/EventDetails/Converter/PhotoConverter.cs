using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UTraveller.Common.Converter;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.Converter
{
    public class PhotoConverter : IConverter<Photo, PhotoEntity>
    {
        private IConverter<BitmapImage, byte[]> imageToBytesConverter;

        public PhotoConverter(IConverter<BitmapImage, byte[]> imageToBytesConverter)
        {
            this.imageToBytesConverter = imageToBytesConverter;
        }

        public PhotoEntity Convert(Photo origin)
        {
            var photo = new PhotoEntity();
            photo.Date = origin.Date;
            photo.Name = origin.Name;
            photo.Thumbnail = imageToBytesConverter.Convert(origin.Thumbnail);
            photo.ImageUrl = origin.ImageUrl;

            return photo;
        }
    }
}
