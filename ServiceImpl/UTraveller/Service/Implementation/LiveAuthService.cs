using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;

namespace UTraveller.Service.Implementation
{
    public class LiveAuthService : ILiveAuthService
    {
        private ISocialClientAccessToken<LiveConnectSession> liveSocialAccessToken;

        public LiveAuthService(ISocialClientAccessToken<LiveConnectSession> liveSocialAccessToken)
        {
            this.liveSocialAccessToken = liveSocialAccessToken;
        }

        public void Logout(string clientId)
        {
            var liveAuthClient = new Microsoft.Live.LiveAuthClient(clientId);
            liveAuthClient.Logout();
            liveSocialAccessToken.AccessToken = null;
        }
    }
}
