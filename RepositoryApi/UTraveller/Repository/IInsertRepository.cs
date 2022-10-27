using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IInsertRepository<T>
    {
        void Insert(T entity);
    }
}
