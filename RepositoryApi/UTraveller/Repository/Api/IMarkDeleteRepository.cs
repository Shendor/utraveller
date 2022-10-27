using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IMarkDeleteRepository<T> where T : IRemotableEntity<long>
    {
        void MarkAsDeleted(T entity);

        IEnumerable<T> GetAllIncludedMarkedAsDeleted(long eventId);
    }
}
