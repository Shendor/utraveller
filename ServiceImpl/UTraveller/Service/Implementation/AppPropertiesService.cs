using RepositoryApi.UTraveller.Repository.Api;
using ServiceImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Mapper;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTraveller.Service.Implementation
{
    public class AppPropertiesService : IAppPropertiesService
    {
        private IWebService webService;
        private IAppPropertiesRepository appPropertiesRepository;
        private IModelMapper<AppProperties, AppPropertiesEntity> propertiesMapper;
        private IModelMapper<AppProperties, UserSettingRemoteModel> userSettingsRemoteMapper;


        public AppPropertiesService(IAppPropertiesRepository appPropertiesRepository,
            IModelMapper<AppProperties, AppPropertiesEntity> propertiesMapper,
            IModelMapper<AppProperties, UserSettingRemoteModel> userSettingsRemoteMapper,
            IWebService webService)
        {
            this.appPropertiesRepository = appPropertiesRepository;
            this.propertiesMapper = propertiesMapper;
            this.userSettingsRemoteMapper = userSettingsRemoteMapper;
            this.webService = webService;
        }


        public AppProperties GetPropertiesForUser(long userId)
        {
            var propertiesEntity = appPropertiesRepository.GetPropertiesForUser(userId);
            if (propertiesEntity == null)
            {
                propertiesEntity = propertiesMapper.MapModel(new AppProperties());
                propertiesEntity.UserId = userId;
                appPropertiesRepository.Insert(propertiesEntity);
            }
            return propertiesMapper.MapEntity(appPropertiesRepository.GetPropertiesForUser(userId));
        }


        public void UpdateProperties(AppProperties appProperties, User user)
        {
            var propertiesEntity = appPropertiesRepository.GetById(appProperties.Id);
            if (propertiesEntity != null)
            {
                var mappedEntity = propertiesMapper.MapModel(appProperties);
                propertiesEntity.MainColor = mappedEntity.MainColor;
                propertiesEntity.BackgroundColor = mappedEntity.BackgroundColor;
                propertiesEntity.TextColor = mappedEntity.TextColor;
                propertiesEntity.CoverOpacity = mappedEntity.CoverOpacity;
                propertiesEntity.PanelsOpacity = mappedEntity.PanelsOpacity;
                propertiesEntity.IsLandscapeCover = mappedEntity.IsLandscapeCover;
                propertiesEntity.IsUploadToFacebook = mappedEntity.IsUploadToFacebook;
                propertiesEntity.IsConnectToServerAutomatically = mappedEntity.IsConnectToServerAutomatically;
                propertiesEntity.IsAllowGeoPosition = mappedEntity.IsAllowGeoPosition;

                appPropertiesRepository.Update(propertiesEntity);
            }
        }


        public async Task RefreshPropertiesFromServer(User user)
        {
            var url = string.Format(ServiceResources.REST_Get_User_Settings, user.RESTAccessToken);
            var result = await webService.GetAsync<RemoteModel<UserSettingRemoteModel>>(url);

            if (result != null && result.ResponseObject != null)
            {
                var propertiesEntity = appPropertiesRepository.GetPropertiesForUser(user.Id);
                if (propertiesEntity != null)
                {
                    propertiesEntity.LimitationCode = result.ResponseObject.LimitCode;
                    appPropertiesRepository.Update(propertiesEntity);
                }
            }
        }


        public async Task<bool> IsVersionUpToDate(string version)
        {
            var result = await webService.GetAsync<RemoteModel<string>>(ServiceResources.REST_Get_AppInfo);

            if (result != null && result.ResponseObject != null)
            {
                return result.ResponseObject != null && result.ResponseObject.Equals(version);
            }
            return true;
        }
    }
}
