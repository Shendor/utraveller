using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository.Implementation
{
    public class PhotoRepository : BaseRemotableEntityRepository<PhotoEntity>, IPhotoRepository
    {
        public PhotoRepository(LocalDatabase database) : base(database) { }

        public IEnumerable<PhotoEntity> GetPhotosOfEvent(long eventId)
        {
            return from PhotoEntity poe in database.Photos
                   where poe.EventId == eventId && !poe.IsDeleted
                   select poe;
        }

        public override void Insert(PhotoEntity entity)
        {
            InsertEntityInTable(database.Photos, entity);
        }

        public override void Delete(PhotoEntity entity)
        {
            DeleteEntityFromTable(database.Photos, entity);
        }

        public PhotoEntity GetById(long id)
        {
            return GetById(database.Photos, id);
        }

        public void DeleteFromEvent(EventEntity eventEntity)
        {
            database.Photos.DeleteAllOnSubmit(from PhotoEntity photo in database.Photos
                                              where photo.Event.Id.Equals(eventEntity.Id)
                                              select photo);
        }


        public void UpdateLocation(ICollection<long> photosId, double latitude, double longitude)
        {
            if (photosId.Count > 0)
            {
                foreach (var photoId in photosId)
                {
                    var entity = GetById(photoId);
                    if (entity != null)
                    {
                        entity.IsSync = false;
                        entity.Latitude = latitude;
                        entity.Longitude = longitude;
                    }
                    database.SubmitChanges();
                }

            }
        }

        public int GetPhotosQuantity(long userId)
        {
            return (from PhotoEntity poe in database.Photos
                    where poe.Event.UserId == userId && !poe.IsDeleted
                    select poe).Count();
        }


        public IEnumerable<PhotoEntity> GetUnSyncPhotosOfEvent(long eventId)
        {
            return from PhotoEntity poe in database.Photos
                   where poe.EventId == eventId && (poe.IsSync == false || poe.IsDeleted == true)
                   select poe;
        }


        public int GetPhotosQuantityOfEvent(long eventId)
        {
            return (from PhotoEntity poe in database.Photos
                    where poe.Event.Id == eventId && !poe.IsDeleted
                    select poe).Count();
        }


        public override IEnumerable<PhotoEntity> GetAllIncludedMarkedAsDeleted(long eventId)
        {
            return from PhotoEntity poe in database.Photos
                   where poe.EventId == eventId
                   select poe;
        }
    }
}
