using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Converter;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Mapper
{
    public class UserMapper : IModelMapper<User, UserEntity>
    {
        public User MapEntity(UserEntity entity)
        {
            var user = new User();
            user.Id = entity.Id;
            user.RemoteId = entity.RemoteId;
            user.IsSync = entity.IsSync;
            user.Name = entity.Name;
            user.Email = entity.Email;
            user.Description = entity.Description;
            user.AvatarContent = entity.Avatar;
            user.CoverContent = entity.Cover;

            return user;
        }


        public UserEntity MapModel(User user)
        {
            var userEntity = new UserEntity();
            userEntity.Id = user.Id;
            userEntity.RemoteId = user.RemoteId;
            userEntity.IsSync = user.IsSync;
            userEntity.Name = user.Name;
            userEntity.Email = user.Email;
            userEntity.Description = user.Description;
            userEntity.Avatar = user.AvatarContent;
            userEntity.Cover = user.CoverContent;

            return userEntity;
        }
    }
}
