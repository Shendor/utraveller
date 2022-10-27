using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IPhotoRepository : IBaseRepository<PhotoEntity, long>, IDeleteFromEventRepository, 
        IMarkDeleteRepository<PhotoEntity>
    {
        int GetPhotosQuantity(long userId);

        int GetPhotosQuantityOfEvent(long eventId);

        IEnumerable<PhotoEntity> GetPhotosOfEvent(long eventId);

        void UpdateLocation(ICollection<long> photosId, double latitude, double longitude);

        IEnumerable<PhotoEntity> GetUnSyncPhotosOfEvent(long eventId);
    }
}
