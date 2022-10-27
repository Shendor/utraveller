using RepositoryApi.UTraveller.Repository.Api;
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
    public class LocalUserService : IUserService
    {
        private IUserRepository userRepository;
        private IAppPropertiesService appPropertiesService;
        private IModelMapper<User, UserEntity> userMapper;
        private User currentUser;

        public LocalUserService(IUserRepository userRepository, IModelMapper<User, UserEntity> userMapper,
            IAppPropertiesService appPropertiesService)
        {
            this.userRepository = userRepository;
            this.userMapper = userMapper;
            this.appPropertiesService = appPropertiesService;
        }

        public Task<string> RefreshAccessToken(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<User> TryAuthenticateCurrentUser()
        {
            var userEntity = userRepository.GetCurrentSignInUser();
            if (userEntity != null)
            {
                currentUser = userMapper.MapEntity(userRepository.GetById(userEntity.Id));
            }
            return currentUser;
        }

        public Task<bool> AuthenticateUserThroughFacebook(string email, string fbAccessToken)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AuthenticateUser(string email, string password)
        {
            UserEntity userEntity = userRepository.GetUserByEmail(email);
            if (userEntity != null)
            {
                userRepository.RegisterCurrentUser(userEntity);
                currentUser = userMapper.MapEntity(userEntity);
            }
            return userEntity != null;
        }

        public async Task<RemoteModel<long?>> RegisterUser(User user, string password)
        {
            user.Email = "default_user@mail.com";
            var userEntity = userRepository.GetUserByEmail(user.Email);
            if (userEntity == null)
            {
                userEntity = userMapper.MapModel(user);
                userEntity.Password = password;
                userEntity.RemoteId = 0;
                userEntity.Name = user.Name == null ? "Unknown" : user.Name;
                userEntity.Description = user.Description == null ? "..." : user.Description;
                userEntity.IsSync = true;
                userRepository.Insert(userEntity);
            }

            return null;
        }

        public void LogOutCurrentUser()
        {
            currentUser = null;
        }

        public User GetCurrentUser()
        {
            return currentUser;
        }

        public bool IsAuthenticated()
        {
            return currentUser != null;
        }

        public void UpdateUser(User user)
        {
            var userEntity = userRepository.GetById(user.Id);
            userEntity.Avatar = user.AvatarContent;
            userEntity.Cover = user.CoverContent;
            userEntity.Name = user.Name;
            userEntity.Description = user.Description;
            userEntity.IsSync = user.IsSync;
            userRepository.Update(userEntity);
        }

        public bool IsUsernameExists(string username)
        {
            return userRepository.IsEmailExist(username);
        }

        public Task<bool> RequestResetPassword(string email)
        {
            throw new NotImplementedException();
        }
    }
}
