using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IUserRepository : IBaseRepository<UserEntity, long>
    {
        UserEntity GetUserByEmail(string email, string password);

        UserEntity GetUserByEmail(string email);

        bool IsEmailExist(string email);

        UserEntity GetCurrentSignInUser();

        void RegisterCurrentUser(UserEntity userEntity);
    }
}
