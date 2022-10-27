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
    public class MessageRemoteModelMapper : IModelMapper<Message, MessageRemoteModel>
    {

        public Message MapEntity(MessageRemoteModel entity)
        {
            var message = new Message();
            message.RemoteId = entity.Id;
            message.Text = entity.Text;
            if (entity.Coordinate != null)
            {
                message.Coordinate = new GeoCoordinate(entity.Coordinate.Lat, entity.Coordinate.Lng);
            }
            message.Date = entity.Date;
            message.FacebookPostId = entity.FacebookPostId;
            message.ChangeDate = entity.ChangeDate;
            return message;
        }


        public MessageRemoteModel MapModel(Message model)
        {
            var messageRemoteModel = new MessageRemoteModel();
            messageRemoteModel.Id = model.RemoteId;
            messageRemoteModel.Text = model.Text;
            messageRemoteModel.ChangeDate = model.ChangeDate;
            messageRemoteModel.FacebookPostId = model.FacebookPostId;
            if (model.Coordinate != null)
            {
                messageRemoteModel.Coordinate = new GeoLocationModel(model.Coordinate.Latitude, model.Coordinate.Longitude);
            }
            messageRemoteModel.Date = new DateTime(model.Date.Ticks, DateTimeKind.Utc);

            return messageRemoteModel;
        }
    }
}
