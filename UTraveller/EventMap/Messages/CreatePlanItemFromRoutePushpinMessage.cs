using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.Messages
{
    public class CreatePlanItemFromRoutePushpinMessage
    {
        public CreatePlanItemFromRoutePushpinMessage(RoutePushpin routePushpin)
        {
            this.RoutePushpin = routePushpin;
        }


        public RoutePushpin RoutePushpin
        {
            get;
            private set;
        }
    }

    public class LaunchCreatePlanItemFromRoutePushpinPageMessage
    {
        public LaunchCreatePlanItemFromRoutePushpinPageMessage(RoutePushpin routePushpin)
        {
            this.RoutePushpin = routePushpin;
        }


        public RoutePushpin RoutePushpin
        {
            get;
            private set;
        }
    }
}
