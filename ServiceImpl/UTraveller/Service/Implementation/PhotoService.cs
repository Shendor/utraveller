using ExifLib;
using RepositoryApi.UTraveller.Repository.Api;
using ServiceApi.UTraveller.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Implementation.Internal;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Mapper;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Implementation
{
    public class PhotoService : BaseCacheableEntityService, IPhotoService
    {
        private IPhotoRepository photoRepository;
        private IModelMapper<Photo, PhotoEntity> photoMapper;
        private IImageLoaderService mediaLibraryPhotoLoaderService;
        private IAppPropertiesService appPropertiesService;

        public PhotoService(IPhotoRepository photoRepository,
            IModelMapper<Photo, PhotoEntity> photoMapper, IImageLoaderService mediaLibraryPhotoLoaderService,
            IAppPropertiesService appPropertiesService)
        {
            this.mediaLibraryPhotoLoaderService = mediaLibraryPhotoLoaderService;
            this.photoRepository = photoRepository;
            this.photoMapper = photoMapper;
            this.appPropertiesService = appPropertiesService;
        }

        public async Task<bool> AddPhoto(Photo photo, Event e)
        {
            if (!IsLimitExceeded(e, photoRepository.GetPhotosQuantityOfEvent(e.Id) + 1))
            {
                try
                {
                    await FindLocationAndAddPhotoLocally(photo, e);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Cannot add photo to event. Error: " + ex.Message);
                    throw ex;
                }
                finally
                {
                    photo.Dispose();
                }
                return true;
            }
            return false;
        }


        public void DeletePhoto(Photo photo, Event e)
        {
            var photoEntity = photoRepository.GetById(photo.Id);
            if (photoEntity != null)
            {
                photoRepository.Delete(photoEntity);
            }
        }


        public IEnumerable<Photo> GetPhotos(Event e)
        {
            return GetPhotosLocally(e); 
        }


        public void ChangeDescription(long id, string description)
        {
            var entity = photoRepository.GetById(id);
            if (entity != null)
            {
                entity.Description = description;
                photoRepository.Update(entity);
            }
        }


        public void UpdatePhotosLocations(ICollection<Photo> photos, Event e, GeoCoordinate location)
        {
            double latitude = location == null ? 0 : location.Latitude;
            double longitude = location == null ? 0 : location.Longitude;
            var photoEntities = new List<long>();
            foreach (var photo in photos)
            {
                photo.IsSync = false;
                photo.Coordinate = location;
                photoEntities.Add(photo.Id);
            }
            photoRepository.UpdateLocation(photoEntities, latitude, longitude);
        }


        public int GetPhotosQuantity(long userId)
        {
            return photoRepository.GetPhotosQuantity(userId);
        }


        public void UpdatePhoto(Photo photo, Event e)
        {
            UpdatePhotoLocally(photo, e);
        }


        private async Task FindLocationAndAddPhotoLocally(Photo photo, Event e)
        {
            var photoEntity = photoMapper.MapModel(photo);
            await TryUpdateLocationForPhoto(photo, photoEntity, e);
            photoEntity.EventId = e.Id;
            photoEntity.IsSync = photo.ImageUrl != null & photo.IsSync;
            photoRepository.Insert(photoEntity);
            photo.Id = photoEntity.Id;
        }


        private void AddPhotoLocally(Photo photo, Event e)
        {
            var photoEntity = photoMapper.MapModel(photo);
            photoEntity.EventId = e.Id;
            photoEntity.IsSync = photo.ImageUrl != null & photo.IsSync;
            photoRepository.Insert(photoEntity);
            photo.Id = photoEntity.Id;
        }


        public Type GetEntityType()
        {
            return typeof(Photo);
        }


        private bool UpdatePhotoLocally(Photo photo, Event e)
        {
            var entity = photoRepository.GetById(photo.Id);
            if (entity != null && entity.EventId == e.Id)
            {
                entity.Description = photo.Description;
                entity.Date = photo.Date;
                entity.ChangeDate = photo.ChangeDate;
                entity.FacebookPostId = photo.FacebookPostId;
                entity.FacebookPhotoId = photo.FacebookPhotoId;
                entity.ImageUrl = photo.ImageUrl;
                entity.IsSync = photo.ImageUrl != null & photo.IsSync;
                if (photo.Coordinate != null)
                {
                    entity.Latitude = photo.Coordinate.Latitude;
                    entity.Longitude = photo.Coordinate.Longitude;
                }
                else
                {
                    entity.Latitude = 0;
                    entity.Longitude = 0;
                }
                photoRepository.Update(entity);
                return true;
            }
            return false;
        }


        private async Task<bool> TryUpdateLocationForPhoto(Photo photo, PhotoEntity photoEntity, Event e)
        {
            bool isLocationFound = false;
            var properties = appPropertiesService.GetPropertiesForUser(e.UserId);
            if (properties != null && !properties.IsAllowGeoPosition)
            {
                return isLocationFound;
            }

            await Task.Run(() =>
            {
                try
                {
                    if (photo.ImageStream == null)
                    {
                        photo.ImageStream = mediaLibraryPhotoLoaderService.Load(photo).Result;
                    }
                    if (photo.ImageStream != null)
                    {
                        using (var stream = new MemoryStream())
                        {
                            photo.ImageStream.Position = 0;
                            photo.ImageStream.CopyTo(stream);
                            stream.Position = 0;
                            using (ExifReader reader = new ExifReader(stream))
                            {
                                double[] longituteArray = new double[3];
                                double[] latitudeArray = new double[3];

                                bool hasLongitude = reader.GetTagValue<double[]>(ExifTags.GPSLongitude, out longituteArray);
                                bool hasLatitude = reader.GetTagValue<double[]>(ExifTags.GPSLatitude, out latitudeArray);

                                if (hasLongitude && hasLatitude)
                                {
                                    UpdateLocationForPhoto(photo, photoEntity, longituteArray, latitudeArray);
                                    isLocationFound = true;
                                }
                            }
                        }
                        photo.ImageStream.Position = 0;
                    }
                }
                catch (ExifLibException exifEx)
                {
                    Debug.WriteLine(exifEx.Message);
                }
            });
            return isLocationFound;
        }


        private ICollection<Photo> GetPhotosLocally(Event e)
        {
            var photos = new SortedSet<Photo>();
            foreach (var photoEntity in photoRepository.GetPhotosOfEvent(e.Id))
            {
                photos.Add(photoMapper.MapEntity(photoEntity));
            }
            return photos;
        }


        private void UpdateLocationForPhoto(Photo photo, PhotoEntity photoEntity, double[] longituteArray, double[] latitudeArray)
        {
            double longitute = longituteArray[0] + (longituteArray[1] / 60) + (longituteArray[2] / (60 * 60));
            double latitude = latitudeArray[0] + (latitudeArray[1] / 60) + (latitudeArray[2] / (60 * 60));
            photo.Coordinate = new GeoCoordinate(latitude, longitute);
            photoEntity.Latitude = (float)latitude;
            photoEntity.Longitude = (float)longitute;
        }


        private bool IsLimitExceeded(Event e, int photosQuantity)
        {
            var properties = appPropertiesService.GetPropertiesForUser(e.UserId);
            if (properties.Limitation.PhotosLimit < photosQuantity)
            {
                throw new LimitExceedException(properties.Limitation.PhotosLimit, "Photo");
            }
            else
            {
                return false;
            }
        }
    }
}
