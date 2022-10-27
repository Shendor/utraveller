using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IPhotoUploader
    {
        Task<IDictionary<string, ICollection<Photo>>> LoadPhotos();

        Photo GetCameraPhotoByName(string name);
    }
}
