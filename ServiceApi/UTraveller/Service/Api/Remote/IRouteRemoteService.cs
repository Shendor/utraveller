using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api.Remote
{
    public interface IRouteRemoteService
    {
        Task<bool> AddRoute(Route route, Event e);

        Task<bool> DeleteRoute(Route route, Event e);

        Task<IEnumerable<Route>> GetRoutes(Event e);
    }
}
