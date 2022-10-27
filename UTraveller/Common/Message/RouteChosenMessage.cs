using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.Message
{
    public class RouteChosenMessage : ObjectsCollectionMessage<Route>
    {
        public RouteChosenMessage(ICollection<Route> routes)
            : base(routes)
        {
        }
    }
}
