using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTraveller.Service.Implementation.Remote
{
    public abstract class BaseRemoteService
    {
        protected bool hasResponseWithoutErrors<T>(RemoteModel<T> result)
        {
            return result != null && result.ResponseObject != null && result.Error == null;
        }
    }
}
