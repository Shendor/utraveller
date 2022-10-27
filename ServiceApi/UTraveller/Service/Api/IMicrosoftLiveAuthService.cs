using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTraveller.Service.Api
{
    public interface IMicrosoftLiveAuthService
    {
        Task<bool> SignIn(string clientId);

        Task<bool> TrySignIn(string clientId);

        void Logout(string clientId);

        bool IsSignedIn();
    }
}
