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
    public class PhotoModelMapper : IModelMapper<Photo, PhotoEntity>
    {
        public Photo MapEntity(PhotoEntity entity)
        {
            var bytesToImageConverter = new BytesToBitmapImageConverter();

            var photo = new Photo();
            photo.Id = entity.Id;
            photo.RemoteId = entity.RemoteId;
            photo.IsSync = entity.IsSync;
            photo.IsDeleted = entity.IsDeleted;
            photo.Date = entity.Date;
            photo.Name = entity.Name;
            photo.Width = entity.Width;
            photo.Height = entity.Height;
            photo.Description = entity.Description;
            photo.ImageUrl = entity.ImageUrl;
            photo.FacebookPostId = entity.FacebookPostId;
            photo.FacebookPhotoId = entity.FacebookPhotoId;
            photo.ChangeDate = entity.ChangeDate;
            if (entity.Latitude != 0 && entity.Longitude != 0)
            {
                photo.Coordinate = new GeoCoordinate(entity.Latitude, entity.Longitude);
            }
            photo.ThumbnailContent = entity.Thumbnail;

            return photo;
        }


        public PhotoEntity MapModel(Photo model)
        {
            var imageToBytesConverter = new BitmapImageToBytesConverter();

            var photoEntity = new PhotoEntity();
            photoEntity.Id = model.Id;
            photoEntity.RemoteId = model.RemoteId;
            photoEntity.IsSync = model.IsSync;
            photoEntity.IsDeleted = model.IsDeleted;
            photoEntity.Date = model.Date;
            photoEntity.Name = model.Name;
            photoEntity.Width = model.Width;
            photoEntity.Height = model.Height;
            photoEntity.Description = model.Description;
            photoEntity.ImageUrl = model.ImageUrl;
            photoEntity.FacebookPostId = model.FacebookPostId;
            photoEntity.FacebookPhotoId = model.FacebookPhotoId;
            photoEntity.ChangeDate = model.ChangeDate;
            if (model.Coordinate != null)
            {
                photoEntity.Latitude = model.Coordinate.Latitude;
                photoEntity.Longitude = model.Coordinate.Longitude;
            }
            photoEntity.Thumbnail = model.ThumbnailContent;

            return photoEntity;
        }
    }
}
