using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Api;
using UTraveller.Service.Model;
using UTraveller.Social.Messages;

namespace UTraveller.Social.ViewModel
{
    public class SocialAuthViewModel : BaseViewModel
    {
        private IFacebookAuthService facebookAuthService;

        public SocialAuthViewModel(IFacebookAuthService facebookAuthService)
        {
            this.facebookAuthService = facebookAuthService;
        }


        public override void Cleanup()
        {
            base.Cleanup();
            Token = null;
        }


        public bool Login(Uri uri)
        {
            var isSignedIn = facebookAuthService.Login(uri, App.CreateFacebookAppData());
            MessengerInstance.Send<FacebookSignedInMessage>(new FacebookSignedInMessage(isSignedIn), Token);
            return isSignedIn;
        }


        public object Token
        {
            get;
            set;
        }


        public Uri CreateAuthUri()
        {
            return facebookAuthService.GetLoginUrl(App.CreateFacebookAppData());
        }

    }
}
