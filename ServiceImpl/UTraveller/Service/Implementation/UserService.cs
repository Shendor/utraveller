using RepositoryApi.UTraveller.Repository.Api;
using ServiceImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Exceptions;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Mapper;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTraveller.Service.Implementation
{
    public class UserService : IUserService
    {
        private static readonly string AUTH_TOKEN_PATTERN = "facebookauth__{0}__{1}";
        private static readonly string AUTH_TOKEN_REGEXP = @"facebookauth__\S+__\S+";

        private IWebService webService;
        private IUserRepository userRepository;
        private IAppPropertiesService appPropertiesService;
        private IModelMapper<User, UserEntity> userMapper;
        private IModelMapper<User, UserRemoteModel> userRemoteMapper;
        private IModelMapper<User, BaseUserRemoteModel> baseUserRemoteMapper;
        private User currentUser;

        public UserService(IUserRepository userRepository, IModelMapper<User, UserEntity> userMapper,
            IModelMapper<User, UserRemoteModel> userRemoteMapper, IModelMapper<User, BaseUserRemoteModel> baseUserRemoteMapper,
            IWebService webService, IAppPropertiesService appPropertiesService)
        {
            this.userRepository = userRepository;
            this.userMapper = userMapper;
            this.userRemoteMapper = userRemoteMapper;
            this.baseUserRemoteMapper = baseUserRemoteMapper;
            this.webService = webService;
            this.appPropertiesService = appPropertiesService;
        }


        public async Task<User> TryAuthenticateCurrentUser()
        {
            var userEntity = userRepository.GetCurrentSignInUser();
            if (userEntity != null)
            {
                currentUser = userMapper.MapEntity(userRepository.GetById(userEntity.Id));

                if (IsConnectToServerAutomatically(userEntity))
                {
                    string accessToken = null;
                    try
                    {
                        accessToken = await GetRemoteAccessToken(userEntity.Email, userEntity.Password);
                    }
                    catch (WebServiceException wsEx)
                    {
                        System.Diagnostics.Debug.WriteLine("Error getting access token: " + wsEx.Message);
                    }
                    currentUser.RESTAccessToken = accessToken;
                    if (accessToken != null && !userEntity.IsSync)
                    {
                        UpdateUserRemotely(currentUser);
                    }
                    await appPropertiesService.RefreshPropertiesFromServer(currentUser);
                }
            }
            return currentUser;
        }


        public async Task<bool> AuthenticateUserThroughFacebook(string email, string fbAccessToken)
        {
            var authToken = string.Format(AUTH_TOKEN_PATTERN, email, fbAccessToken);
            var accessToken = await GetRemoteAccessToken(authToken, null);
            UserEntity userEntity = null;
            if (accessToken != null)
            {
                userEntity = userRepository.GetUserByEmail(email);
                userEntity = await AuthenticateUser(email, authToken, accessToken, userEntity);
            }
            return userEntity != null;
        }


        public async Task<bool> AuthenticateUser(string email, string password)
        {
            // TODO: decrypt password
            var accessToken = await GetRemoteAccessToken(email, password);
            UserEntity userEntity = null;
            if (accessToken != null)
            {
                userEntity = userRepository.GetUserByEmail(email);
                userEntity = await AuthenticateUser(email, password, accessToken, userEntity);
            }

            return userEntity != null;
        }


        public async Task<RemoteModel<long?>> RegisterUser(User user, string password)
        {
            if (user.Email != null && password != null)
            {
                var id = await RegisterUserRemotely(user, password);

                if (id != null && id.ResponseObject > 0)
                {
                    var userEntity = InsertUserToLocalDatabase(user, password, id.ResponseObject);

                    userRepository.RegisterCurrentUser(userEntity);
                    currentUser = userMapper.MapEntity(userEntity);
                    currentUser.RemoteId = userEntity.RemoteId;

                    currentUser.RESTAccessToken = await GetRemoteAccessToken(user.Email, password);
                }

                return id;
            }
            return null;
        }


        public async Task<string> RefreshAccessToken(User user)
        {
            var userEntity = userRepository.GetById(user.Id);
            if (userEntity != null)
            {
                user.RESTAccessToken = await GetRemoteAccessToken(userEntity.Email, userEntity.Password);
            }
            return null;
        }


        public void UpdateUser(User user)
        {
            user.IsSync = false;
            UpdateUserLocally(user);

            if (user.RESTAccessToken != null)
            {
                UpdateUserRemotely(user);
            }
        }


        public User GetCurrentUser()
        {
            return currentUser;
        }


        public bool IsAuthenticated()
        {
            return currentUser != null;
        }


        public void LogOutCurrentUser()
        {
            currentUser = null;
        }


        public bool IsUsernameExists(string username)
        {
            return userRepository.IsEmailExist(username);
        }


        public async Task<bool> RequestResetPassword(string email)
        {
            var url = string.Format(ServiceResources.REST_Request_Reset_Password, email);
            var result = await webService.PostAsync<RemoteModel<bool?>>(url);
            if (result != null && result.ResponseObject != null)
            {
                return result.ResponseObject.Value;
            }
            return false;
        }


        private void UpdateUserLocally(User user)
        {
            var userEntity = userRepository.GetById(user.Id);
            userEntity.Avatar = user.AvatarContent;
            userEntity.Cover = user.CoverContent;
            userEntity.Name = user.Name;
            userEntity.Description = user.Description;
            userEntity.IsSync = user.IsSync;
            userRepository.Update(userEntity);
        }


        private async Task<string> GetRemoteAccessToken(string email, string password)
        {
            if (IsPasswordMatchFBAuthRegexp(password))
            {
                email = password;
                password = null;
            }
            var url = string.Format(ServiceResources.REST_OAuth2_Token_Address, email, password);
            try
            {
                var result = await webService.GetAsyncWithException<AccessTokenRemoteModel>(url);
                return result != null ? result.Access_token : null;
            }
            catch (Exception ex)
            {
                throw new WebServiceException("Cannot get access token: " + ex.Message);
            }
        }


        private async Task<RemoteModel<UserRemoteModel>> GetUserProfile(string accessToken)
        {
            var url = string.Format(ServiceResources.REST_Get_User, accessToken);
            return await webService.GetAsyncWithException<RemoteModel<UserRemoteModel>>(url);
        }


        private async Task<RemoteModel<long?>> RegisterUserRemotely(User user, string password)
        {
            var url = ServiceResources.REST_Register_User;
            var userRemoteModel = userRemoteMapper.MapModel(user);
            userRemoteModel.Password = password;
            var result = await webService.PostAsync<UserRemoteModel, RemoteModel<long?>>(url, userRemoteModel);

            return result;
        }


        private async void UpdateUserRemotely(User user)
        {
            var url = string.Format(ServiceResources.REST_Update_User, user.RESTAccessToken);
            var userRemoteModel = baseUserRemoteMapper.MapModel(user);
            var result = await webService.PostAsync<BaseUserRemoteModel, RemoteModel<bool?>>(url, userRemoteModel);

            if (result != null && result.ResponseObject.Value)
            {
                var userEntity = userRepository.GetById(user.Id);
                userEntity.IsSync = true;
                userRepository.Update(userEntity);
            }
        }


        private UserEntity InsertUserToLocalDatabase(User user, string password, long? id)
        {
            // TODO: encrypt password
            var userEntity = userMapper.MapModel(user);
            userEntity.Password = password;
            userEntity.RemoteId = id.Value;
            userEntity.Name = user.Name == null ? "Unknown" : user.Name;
            userEntity.Description = user.Description == null ? "..." : user.Description;
            userEntity.IsSync = true;
            userRepository.Insert(userEntity);
            return userEntity;
        }


        private bool IsConnectToServerAutomatically(UserEntity userEntity)
        {
            var property = appPropertiesService.GetPropertiesForUser(userEntity.Id);
            return property == null || property.IsConnectToServerAutomatically;
        }


        private async Task<UserEntity> AuthenticateUser(string email, string password, string accessToken, UserEntity userEntity)
        {
            if (accessToken != null && userEntity == null)
            {
                var userProfile = await GetUserProfile(accessToken);
                if (userProfile != null && userProfile.ResponseObject != null)
                {
                    var user = userRemoteMapper.MapEntity(userProfile.ResponseObject);
                    user.Email = email;
                    userEntity = InsertUserToLocalDatabase(user, password, user.RemoteId);
                }
            }
            if (userEntity != null)
            {
                if (!IsPasswordMatchFBAuthRegexp(password))
                {
                    userEntity.Password = password;
                    userRepository.Update(userEntity);
                }
                userRepository.RegisterCurrentUser(userEntity);
                currentUser = userMapper.MapEntity(userEntity);
                if (IsConnectToServerAutomatically(userEntity))
                {
                    currentUser.RESTAccessToken = accessToken;
                    await appPropertiesService.RefreshPropertiesFromServer(currentUser);
                }
            }
            return userEntity;
        }


        private bool IsPasswordMatchFBAuthRegexp(string password)
        {
            Regex regex = new Regex(AUTH_TOKEN_REGEXP);
            return password != null && regex.Match(password).Success;
        }
    }
}
