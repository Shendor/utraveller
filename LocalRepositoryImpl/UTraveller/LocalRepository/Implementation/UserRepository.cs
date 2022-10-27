using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository.Implementation
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(LocalDatabase database) : base(database) { }


        public override void Insert(UserEntity entity)
        {
            InsertEntityInTable(database.Users, entity);
        }


        public override void Delete(UserEntity entity)
        {
            DeleteEntityFromTable(database.Users, entity);
        }


        public override void Update(UserEntity entity)
        {
            var user = GetById(database.Users, entity.Id);
            user.Id = entity.Id;
            user.Name = entity.Name;
            user.Email = entity.Email;
            user.Description = entity.Description;
            user.Avatar = entity.Avatar;
            user.Cover = entity.Cover;

            base.Update(user);
        }


        public UserEntity GetUserByEmail(string email, string password)
        {
            var userEntity = (from u in database.Users
                              where u.Email.Equals(email) && u.Password.Equals(password)
                             select u).FirstOrDefault();
            return userEntity;
        }


        public UserEntity GetById(long id)
        {
            return GetById(database.Users, id);
        }


        public UserEntity GetCurrentSignInUser()
        {
            var registryEntity = (from registry in database.AuthenticationRegistry
                                  orderby registry.SignInDate descending
                                  select registry).FirstOrDefault();
            if (registryEntity != null)
            {
                return GetUserByEmail(registryEntity.Email, registryEntity.Password);
            }
            return null;
        }


        public void RegisterCurrentUser(UserEntity userEntity)
        {
            //TODO: delete all registry entries and insert one
            var registry = new AuthenticationRegistryEntity();
            registry.Email = userEntity.Email;
            registry.Password = userEntity.Password;

            database.AuthenticationRegistry.InsertOnSubmit(registry);
            database.SubmitChanges();
        }


        public bool IsEmailExist(string email)
        {
            return (from u in database.Users
                    where u.Email.Equals(email)
                    select u).FirstOrDefault() != null;
        }


        public UserEntity GetUserByEmail(string email)
        {
            var userEntity = (from u in database.Users
                              where u.Email.Equals(email)
                              select u).FirstOrDefault();
            return userEntity;
        }
    }
}
