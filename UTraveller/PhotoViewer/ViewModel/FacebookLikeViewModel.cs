using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Model;

namespace UTraveller.PhotoViewer.ViewModel
{
    public class FacebookLikeViewModel : BaseViewModel
    {
        private FacebookUserProfile userProfile;

        public FacebookLikeViewModel(FacebookUserProfile userProfile)
        {
            this.userProfile = userProfile;
        }

        public string From
        {
            get { return userProfile.Name; }
        }


        public string AvatarUrl
        {
            get { return userProfile.ImageUrl; }
        }
    }
}
