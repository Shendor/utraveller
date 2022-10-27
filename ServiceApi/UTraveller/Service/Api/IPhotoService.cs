using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IPhotoService
    {
        int GetPhotosQuantity(long userId);

        IEnumerable<Photo> GetPhotos(Event e);

        Task<bool> AddPhoto(Photo photo, Event e);

        void DeletePhoto(Photo photo, Event e);

        void UpdatePhoto(Photo photo, Event e);

        void ChangeDescription(long id, string description);

        void UpdatePhotosLocations(ICollection<Photo> photos, Event e, GeoCoordinate location);

    }
}
