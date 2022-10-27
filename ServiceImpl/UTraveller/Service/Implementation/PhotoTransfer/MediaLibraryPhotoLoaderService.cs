using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class MediaLibraryPhotoLoaderService : IImageLoaderService
    {

        public async Task<Stream> Load(Photo photo)
        {
            Stream imageStream = null;
            using (var mediaLibrary = new MediaLibrary())
            {
                foreach (var picture in mediaLibrary.Pictures)
                {
                    if (picture.Name.Equals(photo.Name) &&
                        picture.Date.TimeOfDay.Seconds.Equals(photo.Date.TimeOfDay.Seconds))
                    {
                        imageStream = picture.GetImage();
                        break;
                    }
                }
            }
            return imageStream;
        }

    }
}
