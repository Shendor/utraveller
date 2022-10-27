using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Service.Api
{
    public interface IRouteService
    {
        void AddRoute(Route route, Event e);

        void DeleteRoute(Route route, Event e);

        IEnumerable<Route> GetRoutes(Event e);

        bool InitializeRouteData(Route route);
    }
}
