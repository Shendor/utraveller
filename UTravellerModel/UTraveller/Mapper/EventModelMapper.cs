using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Mapper
{
    public class EventModelMapper : IModelMapper<Event, EventEntity>
    {
        public Event MapEntity(EventEntity entity)
        {
            var e = new Event();
            e.Id = entity.Id;
            e.RemoteId = entity.RemoteId;
            e.UserId = entity.UserId;
            e.Name = entity.Name;
            e.Image = entity.Image == null || entity.Image.Length == 0 ? null : entity.Image;
            e.Date = entity.StartDate;
            e.EndDate = entity.EndDate;
            e.PhotosQuantity = entity.PhotosQuantity;
            e.IsCurrent = entity.IsCurrent;
            e.IsDeleted = entity.IsDeleted;
            e.IsSync = entity.IsSync;
            e.ChangeDate = entity.ChangeDate;

            return e;
        }


        public EventEntity MapModel(Event model)
        {
            var eventEntity = new EventEntity();
            eventEntity.RemoteId = model.RemoteId;
            eventEntity.UserId = model.UserId;
            eventEntity.Name = model.Name;
            eventEntity.Image = model.Image == null || model.Image.Length == 0 ? null : model.Image;
            eventEntity.StartDate = model.Date;
            eventEntity.EndDate = model.EndDate;
            eventEntity.PhotosQuantity = model.PhotosQuantity;
            eventEntity.IsCurrent = model.IsCurrent;
            eventEntity.IsDeleted = model.IsDeleted;
            eventEntity.IsSync = model.IsSync;
            eventEntity.ChangeDate = model.ChangeDate;

            return eventEntity;
        }
    }
}
