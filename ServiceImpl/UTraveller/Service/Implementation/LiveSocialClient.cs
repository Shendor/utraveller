using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;

namespace UTraveller.Service.Implementation
{
    public class LiveSocialClient : ISocialClientAccessToken<LiveConnectSession>
    {
        public LiveConnectSession AccessToken
        {
            get;
            set;
        }
    }
}
