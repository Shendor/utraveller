using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Converter;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Mapper
{
    public class PushpinModelMapper : IModelMapper<PhotoPushpin, PushpinEntity>
    {
        public PhotoPushpin MapEntity(PushpinEntity entity)
        {
            var bytesToImageConverter = new BytesToBitmapImageConverter();

            var photoPushpin = new PhotoPushpin();
            photoPushpin.Id = entity.Id;
            photoPushpin.Description = entity.Description;
            photoPushpin.Coordinate = new GeoCoordinate(entity.Latitude, entity.Longitude);
            photoPushpin.Thumbnail = bytesToImageConverter.Convert(entity.Thumbnail);

            return photoPushpin;
        }

        public PushpinEntity MapModel(PhotoPushpin model)
        {
            var imageToBytesConverter = new BitmapImageToBytesConverter();

            var pushpinEntity = new PushpinEntity();
            pushpinEntity.Description = model.Description;
            pushpinEntity.Longitude = model.Coordinate.Longitude;
            pushpinEntity.Latitude = model.Coordinate.Latitude;
            pushpinEntity.Thumbnail = imageToBytesConverter.Convert(model.Thumbnail);

            return pushpinEntity;
        }
    }
}
