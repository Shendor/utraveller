using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTraveller.Service.Api
{
    public interface IWebService
    {
        Task<T> GetAsyncWithException<T>(string url);

        Task<T> GetAsync<T>(string url);

        Task<Response> PostAsync<Request, Response>(string url, Request postData);

        Task<T> PostAsync<T>(string url);
    }

}
