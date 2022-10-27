using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository.Implementation
{
    public class AppPropertiesRepository : BaseRepository<AppPropertiesEntity>, IAppPropertiesRepository
    {
        public AppPropertiesRepository(LocalDatabase database)
            : base(database)
        {
        }


        public AppPropertiesEntity GetPropertiesForUser(long userId)
        {
            return (from p in database.Properties
                    where p.UserId == userId
                    select p).FirstOrDefault();
        }


        public override void Insert(AppPropertiesEntity entity)
        {
            InsertEntityInTable(database.Properties, entity);
        }


        public AppPropertiesEntity GetById(long id)
        {
            return GetById(database.Properties, id);
        }


        public override void Delete(AppPropertiesEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
