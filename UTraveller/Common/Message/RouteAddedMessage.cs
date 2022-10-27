using ServiceApi.UTraveller.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class RouteAddedMessage : SkyDriveFileAddedMessage
    {
        public RouteAddedMessage(RouteInfo route)
            : base(route)
        {
        }

        public RouteInfo RouteInfo
        {
            get { return Object as RouteInfo; }
        }
    }
}
