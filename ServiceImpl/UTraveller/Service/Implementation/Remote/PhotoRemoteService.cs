using RepositoryApi.UTraveller.Repository.Api;
using ServiceImpl;
using ServiceImpl.UTraveller.Service.Model;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Model;
using UTravellerModel.UTraveller.Mapper;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTraveller.Service.Implementation.Remote
{
    public class PhotoRemoteService : BaseRemoteService, IPhotoRemoteService
    {
        private IPhotoUploadService<FacebookPhotoUploadRequest> photoUploadService;
        private IPhotoRepository photoRepository;
        private IModelMapper<Photo, PhotoRemoteModel> photoRemoteMapper;
        private IWebService webService;
        private IUserService userService;
        private IAppPropertiesService appPropertiesService;

        public PhotoRemoteService(IPhotoRepository photoRepository, IModelMapper<Photo, PhotoRemoteModel> photoRemoteMapper,
            IWebService webService, IUserService userService, IPhotoUploadService<FacebookPhotoUploadRequest> photoUploadService,
            IAppPropertiesService appPropertiesService)
        {
            this.photoRepository = photoRepository;
            this.webService = webService;
            this.photoRemoteMapper = photoRemoteMapper;
            this.userService = userService;
            this.photoUploadService = photoUploadService;
            this.appPropertiesService = appPropertiesService;
        }


        public async Task<IEnumerable<Photo>> GetPhotosOfEvent(Event e)
        {
            List<Photo> photos = null;
            var user = userService.GetCurrentUser();
            if (user.Id == e.UserId && user.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Get_Photos, user.RemoteId, e.RemoteId, user.RESTAccessToken);
                var result = await webService.GetAsync<RemoteModel<IList<PhotoRemoteModel>>>(url);

                if (hasResponseWithoutErrors(result))
                {
                    photos = new List<Photo>();
                    foreach (var item in result.ResponseObject)
                    {
                        photos.Add(photoRemoteMapper.MapEntity(item));
                    }
                }
            }
            return photos;
        }


        public async Task<bool> AddPhotoToEvent(Photo photo, Event e)
        {
            bool isAdded = false;
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
            {
                var properties = appPropertiesService.GetPropertiesForUser(currentUser.Id);
                if (properties != null && properties.IsUploadToFacebook)
                {
                    photo.ImageUrl = await photoUploadService.UploadAndGetUrl(new FacebookPhotoUploadRequest(photo, e.Name));
                }
                var url = string.Format(ServiceResources.REST_Add_Photo, e.RemoteId, currentUser.RESTAccessToken);
                var photoRemoteModel = photoRemoteMapper.MapModel(photo);

                RemoteModel<long?> result = null;
                try
                {
                    result = await webService.PostAsync<PhotoRemoteModel, RemoteModel<long?>>(url, photoRemoteModel);
                }
                catch (Exception ex)
                {
                    photoUploadService.Delete(new FacebookPhotoUploadRequest(photo));
                    throw ex;
                }

                if (result != null && hasResponseWithoutErrors(result))
                {
                    var photoEntity = photoRepository.GetById(photo.Id);
                    photo.RemoteId = photoEntity.RemoteId = result.ResponseObject.Value;
                    photoEntity.IsSync = photo.IsSync = photo.ImageUrl != null;
                    photoEntity.FacebookPhotoId = photo.FacebookPhotoId;
                    photoEntity.ImageUrl = photo.ImageUrl;
                    photoEntity.ChangeDate = photo.ChangeDate = result.ChangeDate;
                    photoRepository.Update(photoEntity);
                    isAdded = true;
                }
                else
                {
                    photoUploadService.Delete(new FacebookPhotoUploadRequest(photo));
                }
            }
            return isAdded;
        }


        public async Task<bool> DeletePhoto(Photo photo, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0 && photo.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Delete_Photo, e.RemoteId, photo.RemoteId, currentUser.RESTAccessToken);
                var result = await webService.PostAsync<RemoteModel<bool?>>(url);
                if (hasResponseWithoutErrors(result) && result.ResponseObject.Value)
                {
                    return await photoUploadService.Delete(new FacebookPhotoUploadRequest(photo));
                }
            }
            return false;
        }


        public async Task<bool> UpdatePhotosLocations(ICollection<Photo> photos, Event e, GeoCoordinate location)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0)
            {
                var url = string.Format(ServiceResources.REST_Update_Photos, e.RemoteId, currentUser.RESTAccessToken);

                var photosRemoteModel = new List<PhotoRemoteModel>();
                foreach (var photo in photos)
                {
                    photosRemoteModel.Add(convertToPhotoRemoteModelAndClearThumbnail(photo));
                }

                var result = await webService.PostAsync<List<PhotoRemoteModel>, RemoteModel<bool?>>(url, photosRemoteModel);

                if (hasResponseWithoutErrors(result) && result.ResponseObject.Value)
                {
                    foreach (var photo in photos)
                    {
                        UpdatePhotoSyncFlag(photo, result.ChangeDate);
                    }
                    return true;
                }
            }
            return false;
        }


        public async Task<bool> UpdatePhoto(Photo photo, Event e)
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null && e.RemoteId > 0 && photo.RemoteId > 0)
            {
                var properties = appPropertiesService.GetPropertiesForUser(currentUser.Id);
                if (photo.ImageUrl == null && properties != null && properties.IsUploadToFacebook)
                {
                    photo.ImageUrl = await photoUploadService.UploadAndGetUrl(new FacebookPhotoUploadRequest(photo, e.Name));
                }

                var url = string.Format(ServiceResources.REST_Update_Photo, e.RemoteId, photo.RemoteId, currentUser.RESTAccessToken);
                var photoRemoteModel = convertToPhotoRemoteModelAndClearThumbnail(photo);
                var result = await webService.PostAsync<PhotoRemoteModel, RemoteModel<bool?>>(url, photoRemoteModel);

                if (hasResponseWithoutErrors(result) && result.ResponseObject.Value)
                {
                    UpdatePhotoSyncFlag(photo, result.ChangeDate);
                    return true;
                }
            }
            return false;
        }


        private PhotoRemoteModel convertToPhotoRemoteModelAndClearThumbnail(Photo photo)
        {
            var photoRemoteModel = photoRemoteMapper.MapModel(photo);
            photoRemoteModel.Thumbnail = null;
            return photoRemoteModel;
        }


        private void UpdatePhotoSyncFlag(Photo photo, DateTime? changeDate)
        {
            var photoEntity = photoRepository.GetById(photo.Id);
            photoEntity.IsSync = photo.IsSync = photo.ImageUrl != null;
            photoEntity.ChangeDate = photo.ChangeDate = changeDate;
            photoRepository.Update(photoEntity);
        }
    }
}
