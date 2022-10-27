using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api.Remote
{
    public interface IPhotoRemoteService 
    {
        Task<IEnumerable<Photo>> GetPhotosOfEvent(Event e);

        Task<bool> AddPhotoToEvent(Photo photo, Event e);

        Task<bool> DeletePhoto(Photo photo, Event e);

        Task<bool> UpdatePhoto(Photo photo, Event e);

        Task<bool> UpdatePhotosLocations(ICollection<Photo> photos, Event e, GeoCoordinate location);
    }
}
