using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class PhotoUploader : IPhotoUploader
    {

        public Photo GetCameraPhotoByName(string name)
        {
            Photo photo = null;
            using (var mediaLibrary = new MediaLibrary())
            {
                PictureAlbumCollection allAlbums = mediaLibrary.RootPictureAlbum.Albums;
                var picture = FindPhoto(allAlbums, "Camera Roll", name);
                if (picture != null)
                {
                    photo = new Photo(picture);
                }
            }

            return photo;
        }


        public async Task<IDictionary<string, ICollection<Photo>>> LoadPhotos()
        {
            return await Task.Run<IDictionary<string, ICollection<Photo>>>(() =>
            {
                var photosInAlbums = new Dictionary<string, ICollection<Photo>>();
                PictureAlbumCollection allAlbums = null;
                using (var mediaLibrary = new MediaLibrary())
                {
                    allAlbums = mediaLibrary.RootPictureAlbum.Albums;

                    foreach (var album in allAlbums)
                    {
                        var photos = GetImagesFromAlbum(album);
                        if (photos.Count > 0)
                        {
                            photosInAlbums.Add(album.Name, photos);
                        }
                    }
                }
                return photosInAlbums;
            });
        }


        private Picture FindPhoto(PictureAlbumCollection albums, string albumName, string name)
        {
            var indexOfLastSlash = name.LastIndexOf("\\");
            if (indexOfLastSlash > 0)
            {
                name = name.Substring(indexOfLastSlash + 1, name.Length - indexOfLastSlash - 1);
            }
            PictureAlbum picturesAlbum = albums.Where(album => album.Name == albumName).FirstOrDefault();
            Picture picture = null;
            if (picturesAlbum != null)
            {
                picture = picturesAlbum.Pictures.Where(pic => pic.Name.Equals(name)).FirstOrDefault();
            }
            return picture;
        }

        private ICollection<Photo> GetImagesFromAlbum(PictureAlbum album)
        {
            var photos = new List<Photo>();
            foreach (var internalAlbum in album.Albums)
            {
                photos.AddRange(GetImagesFromAlbum(internalAlbum));
            }
            foreach (var picture in album.Pictures)
            {
                try
                {
                    var photo = new Photo(picture);
                    photos.Add(photo);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(picture + " cannot be loaded properly: " + ex.Message);
                }
            }

            return photos;
        }

    }
}
