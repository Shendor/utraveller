using Microsoft.Live;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;

namespace UTraveller.Service.Implementation
{
    public class MicrosoftLiveAuthService : IMicrosoftLiveAuthService
    {
        private const string IS_SIGNED_IN_KEY = "is_live_signed_in";

        private ISocialClientAccessToken<LiveConnectSession> liveSocialAccessToken;


        public MicrosoftLiveAuthService(ISocialClientAccessToken<LiveConnectSession> liveSocialAccessToken)
        {
            this.liveSocialAccessToken = liveSocialAccessToken;
        }


        public async Task<bool> SignIn(string clientId)
        {
            try
            {
                var auth = new LiveAuthClient(clientId);
                var loginResult = await auth.LoginAsync(new string[] { "wl.signin", "wl.basic", "wl.photos", "wl.skydrive", "wl.offline_access" });
                
                if (loginResult.Status == LiveConnectSessionStatus.Connected)
                {
                    liveSocialAccessToken.AccessToken = loginResult.Session;
                }
            }
            catch (LiveAuthException ex)
            {
                Debug.WriteLine("Cannot sign in Live acc. Error: " + ex.Message);
            }

            return liveSocialAccessToken.AccessToken != null;
        }


        public async Task<bool> TrySignIn(string clientId)
        {
            return await SignIn(clientId);
        }


        public void Logout(string clientId)
        {
            var auth = new LiveAuthClient(clientId);
            auth.Logout();
            liveSocialAccessToken.AccessToken = null;

            SaveIsSignedInParameter(false);
        }

        public bool IsSignedIn()
        {
            return liveSocialAccessToken.AccessToken != null;
        }

        private static void SaveIsSignedInParameter(bool isSignedIn)
        {
            IsolatedStorageSettings.ApplicationSettings[IS_SIGNED_IN_KEY] = isSignedIn;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

    }
}
