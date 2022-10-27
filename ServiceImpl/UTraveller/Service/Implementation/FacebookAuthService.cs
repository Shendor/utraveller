using Facebook;
using Facebook.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using UTraveller.Service.Api;
using UTraveller.Service.Model;

namespace UTraveller.Service.Implementation
{
    public class FacebookAuthService : IFacebookAuthService
    {
        private const string IS_SIGNED_IN_KEY = "is_facebook_signed_in";
        private const string FACEBOOK_KEY = "facebook_auth_token";

        private ISocialClientAccessToken<string> facebookAccessToken;


        public FacebookAuthService(ISocialClientAccessToken<string> facebookAccessToken)
        {
            this.facebookAccessToken = facebookAccessToken;
        }


        public bool Login(Uri uri, SocialAppData socialAppData)
        {
            DateTime? expireDate = null;
            if (IsAlreadySignedIn())
            {
                facebookAccessToken.AccessToken = (string)IsolatedStorageSettings.ApplicationSettings[FACEBOOK_KEY];
                expireDate = (DateTime?)IsolatedStorageSettings.ApplicationSettings[IS_SIGNED_IN_KEY];
            }
            else if (uri != null)
            {
                try
                {
                    var facebookClient = new FacebookClient();
                    facebookClient.AppId = socialAppData.AppKey;
                    //facebookClient.AppSecret = socialAppData.AppSecret;

                    if (!uri.AbsoluteUri.Contains("access_token"))
                    {
                        uri = new Uri("https://m.facebook.com/?refsrc=https%3A%2F%2Fwww.facebook.com%2F&_rdr");
                    }
                    FacebookOAuthResult oauthResult;
                    if (facebookClient.TryParseOAuthCallbackUrl(uri, out oauthResult))
                    {
                        if (oauthResult.IsSuccess)
                        {
                            facebookAccessToken.AccessToken = oauthResult.AccessToken;
                            expireDate = oauthResult.Expires;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            SaveIsSignedInParameter(expireDate, facebookAccessToken.AccessToken);
            return facebookAccessToken.AccessToken != null;
        }


        public bool TrySignIn(SocialAppData socialAppData)
        {
            if (IsAlreadySignedIn() && facebookAccessToken.AccessToken == null)
            {
                return Login(null, socialAppData);
            }
            else
            {
                return false;
            }
        }


        public Uri GetLoginUrl(SocialAppData socialAppData)
        {
            return new FacebookClient().GetLoginUrl(new
            {
                client_id = socialAppData.AppKey,
                response_type = "token",
                scope = "email,public_profile,publish_actions,user_photos",
                redirect_uri = "https://www.facebook.com/connect/login_success.html"
            });
        }


        public async Task<bool> Logout(SocialAppData socialAppData)
        {
            var facebookClient = new FacebookClient();
            facebookClient.AccessToken = facebookAccessToken.AccessToken;

            var logoutUrl = facebookClient.GetLogoutUrl(new { redirect_uri = @"http://www.facebook.com", access_token = facebookAccessToken.AccessToken });
            try
            {
                await facebookClient.PostTaskAsync(logoutUrl.AbsoluteUri, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fail to logout: " + ex.Message);
            }
            facebookAccessToken.AccessToken = null;
            SaveIsSignedInParameter(null, null);

            return true;
        }


        public bool IsSignedIn()
        {
            return facebookAccessToken.AccessToken != null;
        }


        private static void SaveIsSignedInParameter(DateTime? expireDate, string token)
        {
            IsolatedStorageSettings.ApplicationSettings[IS_SIGNED_IN_KEY] = expireDate;
            IsolatedStorageSettings.ApplicationSettings[FACEBOOK_KEY] = token;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }


        public Uri GetLogoutUrl()
        {
            var facebookClient = new FacebookClient();
            facebookClient.AccessToken = facebookAccessToken.AccessToken;

            return facebookClient.GetLogoutUrl(new
            {
                access_token = facebookClient.AccessToken
            });
        }


        private bool IsAlreadySignedIn()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(IS_SIGNED_IN_KEY) &&
                IsolatedStorageSettings.ApplicationSettings.Contains(FACEBOOK_KEY))
            {
                var expireDate = (DateTime?)IsolatedStorageSettings.ApplicationSettings[IS_SIGNED_IN_KEY];
                return expireDate != null && expireDate > DateTime.Now;
            }
            return false;
        }
    }
}
