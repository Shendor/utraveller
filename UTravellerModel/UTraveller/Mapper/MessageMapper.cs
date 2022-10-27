using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Mapper
{
    public class MessageMapper : IModelMapper<Message, MessageEntity>
    {
        public Message MapEntity(MessageEntity entity)
        {
            var message = new Message();
            message.Id = entity.Id;
            message.IsSync = entity.IsSync;
            message.Date = entity.Date;
            message.Text = entity.Text;
            message.RemoteId = entity.RemoteId;
            message.FacebookPostId = entity.FacebookPostId;
            message.ChangeDate = entity.ChangeDate;
            message.IsDeleted = entity.IsDeleted;
            if (entity.Latitude != 0 && entity.Longitude != 0)
            {
                message.Coordinate = new GeoCoordinate(entity.Latitude, entity.Longitude);
            }

            return message;
        }

        public MessageEntity MapModel(Message model)
        {
            var entity = new MessageEntity();
            entity.Id = model.Id;
            entity.IsSync = model.IsSync;
            entity.Date = model.Date;
            entity.Text = model.Text;
            entity.FacebookPostId = model.FacebookPostId;
            entity.RemoteId = model.RemoteId;
            entity.ChangeDate = model.ChangeDate;
            entity.IsDeleted = model.IsDeleted;
            if (model.Coordinate != null)
            {
                entity.Latitude = model.Coordinate.Latitude;
                entity.Longitude = model.Coordinate.Longitude;
            }

            return entity;
        }
    }
}
