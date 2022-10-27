using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace LocalRepositoryImpl.UTraveller.LocalRepository
{
    public abstract class BaseRemotableEntityRepository<T> : BaseRepository<T>, IMarkDeleteRepository<T>
        where T : class, IRemotableEntity<long>
    {
        public BaseRemotableEntityRepository(LocalDatabase database) : base(database) { }

        public abstract override void Insert(T entity);

        public abstract override void Delete(T entity);

        public abstract IEnumerable<T> GetAllIncludedMarkedAsDeleted(long eventId);

        public void MarkAsDeleted(T entity)
        {
            entity.IsDeleted = true;
            database.SubmitChanges();
        }
    }
}
