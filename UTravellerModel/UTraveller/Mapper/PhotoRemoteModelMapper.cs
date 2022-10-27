using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTravellerModel.UTraveller.Mapper
{
    public class PhotoRemoteModelMapper : IModelMapper<Photo, PhotoRemoteModel>
    {
        public Photo MapEntity(PhotoRemoteModel entity)
        {
            var photo = new Photo();
            photo.RemoteId = entity.Id;
            photo.Width = entity.Width;
            photo.Height = entity.Height;
            photo.Date = entity.Date;
            photo.Description = entity.Description;
            photo.ThumbnailContent = entity.Thumbnail;
            photo.Name = entity.Name;
            photo.FacebookPhotoId = entity.FacebookPhotoId;
            photo.FacebookPostId = entity.FacebookPostId;
            photo.ChangeDate = entity.ChangeDate;
            if (entity.Coordinate != null)
            {
                photo.Coordinate = new GeoCoordinate(entity.Coordinate.Lat, entity.Coordinate.Lng);
            }
            photo.ImageUrl = entity.ImageUrl;

            return photo;
        }

        public PhotoRemoteModel MapModel(Photo model)
        {
            var photoRemoteModel = new PhotoRemoteModel();
            photoRemoteModel.Id = model.RemoteId;
            photoRemoteModel.Width = model.Width;
            photoRemoteModel.Height = model.Height;
            photoRemoteModel.Date = new DateTime(model.Date.Ticks, DateTimeKind.Utc);
            photoRemoteModel.Description = model.Description;
            photoRemoteModel.Thumbnail = model.ThumbnailContent;
            photoRemoteModel.Name = model.Name;
            photoRemoteModel.FacebookPhotoId = model.FacebookPhotoId;
            photoRemoteModel.FacebookPostId = model.FacebookPostId;
            photoRemoteModel.ChangeDate = model.ChangeDate;
            if (model.Coordinate != null)
            {
                photoRemoteModel.Coordinate = new GeoLocationModel(model.Coordinate.Latitude, model.Coordinate.Longitude);
            }
            photoRemoteModel.ImageUrl = model.ImageUrl;

            return photoRemoteModel;
        }
    }
}
