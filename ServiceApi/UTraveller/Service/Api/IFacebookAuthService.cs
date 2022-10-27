using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Model;

namespace UTraveller.Service.Api
{
    public interface IFacebookAuthService
    {
        bool Login(Uri uri, SocialAppData socialAppData);

        bool TrySignIn(SocialAppData socialAppData);

        Uri GetLoginUrl(SocialAppData socialAppData);

        Uri GetLogoutUrl();

        Task<bool> Logout(SocialAppData socialAppData);

        bool IsSignedIn();
    }

    //public delegate void FacebookSuccessAuthHandler();
}
