using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTraveller.Service.Api
{
    public interface IUserService
    {
        Task<string> RefreshAccessToken(User user);

        Task<User> TryAuthenticateCurrentUser();

        Task<bool> AuthenticateUserThroughFacebook(string email, string fbAccessToken);

        Task<bool> AuthenticateUser(string email, string password);

        Task<RemoteModel<long?>> RegisterUser(User user, string password);

        void LogOutCurrentUser();

        User GetCurrentUser();

        bool IsAuthenticated();

        void UpdateUser(User user);

        bool IsUsernameExists(string username);

        Task<bool> RequestResetPassword(string email);
    }
}
