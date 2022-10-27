using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Model;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IFileExplorerService<T> where T : BaseFileExplorerRequest
    {
        Task<ICollection<File>> GetFiles(T searchParameter);

        Task<System.IO.Stream> ReadFile(T searchParameter);
    }
}
