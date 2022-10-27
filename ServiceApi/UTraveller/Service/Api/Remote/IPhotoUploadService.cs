using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api.Remote
{
    public interface IPhotoUploadService<Request>
    {
        Task<string> UploadAndGetUrl(Request request);

        Task<bool> Delete(Request request);
    }
}
