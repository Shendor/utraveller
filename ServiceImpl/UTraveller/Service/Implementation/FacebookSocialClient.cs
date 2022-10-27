using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Service.Api;

namespace UTraveller.Service.Implementation
{
    public class FacebookSocialClient : ISocialClientAccessToken<string>
    {
        public string AccessToken { get; set; }
    }
}
