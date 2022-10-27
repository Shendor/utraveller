using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryApi.UTraveller.Repository.Api
{
    public interface IBaseRepository<T, I> : IInsertRepository<T>, IUpdateRepository<T>, IDeleteRepository<T>
    {
        T GetById(I id);
    }
}
