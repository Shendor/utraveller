using Microsoft.Expression.Interactivity.Core;
using Microsoft.Live;
using Ninject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Control;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Model;
using UTraveller.Social.Messages;

namespace UTraveller.Auth.ViewModel
{
    public class UserSignInViewModel : BaseViewModel
    {
        private static readonly string SOCIAL_AUTH_PAGE = "/Social/SocialAuthBrowserPage.xaml";
        private static readonly string FB_AUTH_TOKEN = "FB_SIGN_IN_TOKEN";

        private NotificationService notificationService;
        private IUserService userService;
        private IFacebookAuthService facebookAuthService;
        private INavigationService navigationService;
        private IFacebookClientService facebookClientService;
        private IWebService webService;
        private ITaskProgressService taskProgressService;
        private ISocialClientAccessToken<string> facebookAccessToken;
        private ILiveAuthService liveAuthService;
        private INetworkConnectionCheckService networkCheckService;

        public UserSignInViewModel(IUserService userService, NotificationService notificationService,
            IFacebookAuthService facebookAuthService, INavigationService navigationService, IFacebookClientService facebookClientService,
            IWebService webService, ICancelableTaskProgressService taskProgressService,
            ISocialClientAccessToken<string> facebookAccessToken, INetworkConnectionCheckService networkCheckService,
            ILiveAuthService liveAuthService)
        {
            this.userService = userService;
            this.notificationService = notificationService;
            this.facebookAuthService = facebookAuthService;
            this.navigationService = navigationService;
            this.facebookClientService = facebookClientService;
            this.webService = webService;
            this.taskProgressService = taskProgressService;
            this.facebookAccessToken = facebookAccessToken;
            this.networkCheckService = networkCheckService;
            this.liveAuthService = liveAuthService;

            SignUpFacebookCommand = new ActionCommand(SignUpFacebook);
            SignInUserCommand = new ActionCommand(SignIn);
            RequestResetPasswordCommand = new ActionCommand(RequestResetPassword);
        }


        public void Initialize()
        {
            MessengerInstance.Unregister<FacebookSignedInMessage>(this);
            MessengerInstance.Register<FacebookSignedInMessage>(this, FB_AUTH_TOKEN, OnFacebookSignedIn);
        }


        public override void Cleanup()
        {
            MessengerInstance.Unregister<FacebookSignedInMessage>(this);
        }


        public ICommand SignUpFacebookCommand
        {
            get;
            private set;
        }


        public ICommand SignInUserCommand
        {
            get;
            private set;
        }


        public ICommand RequestResetPasswordCommand
        {
            get;
            private set;
        }


        public string Username
        {
            get;
            set;
        }


        public string Password
        {
            get;
            set;
        }


        public string EmailToResetPassword
        {
            get;
            set;
        }


        private void SignUpFacebook()
        {
            if (networkCheckService.HasConnection)
            {
                navigationService.Navigate(SOCIAL_AUTH_PAGE, FB_AUTH_TOKEN);
            }
            else
            {
                notificationService.Show(AppResources.No_Connection_To_Sign_In);
            }
        }


        private async void SignIn()
        {
            if (networkCheckService.HasConnection)
            {
                var result = await SignInByEmail(Username, Password, (email, password) => userService.AuthenticateUser(email, password));
                if (result && facebookAuthService.IsSignedIn())
                {
                    await facebookAuthService.Logout(App.CreateFacebookAppData());
                    liveAuthService.Logout(App.LiveClientId);
                }
            }
            else
            {
                notificationService.Show(AppResources.No_Connection_To_Sign_In);
            }
        }


        private async void RequestResetPassword()
        {
            if (string.IsNullOrEmpty(EmailToResetPassword))
            {
                return;
            }

            taskProgressService.RunIndeterminateProgress(AppResources.SignIn_Request_Reset_Password_Progress);
            try
            {
                if (await userService.RequestResetPassword(EmailToResetPassword))
                {
                    notificationService.Show(AppResources.SignIn_Request_Reset_Password_Success);
                }
                else
                {
                    notificationService.Show(AppResources.SignIn_Request_Reset_Password_Fail);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Can't send request to reset password: " + ex.Message);
            }
            finally
            {
                taskProgressService.FinishProgress();
            }
        }


        private async void OnFacebookSignedIn(FacebookSignedInMessage message)
        {
            if (message.Object)
            {
                var profile = await facebookClientService.GetUserProfile();
                await SignInByEmail(profile.Email, facebookAccessToken.AccessToken,
                    (email, password) => userService.AuthenticateUserThroughFacebook(email, password));
            }
        }


        private async Task<bool> SignInByEmail(string email, string password, Func<string, string, Task<bool>> signInFunc)
        {
            taskProgressService.RunIndeterminateProgress(AppResources.SigningIn_User);
            bool result = false;
            try
            {
                var isAuthenticated = await signInFunc(email, password);
                if (!isAuthenticated)
                {
                    notificationService.Show(AppResources.Wrong_Email_Or_Password);
                }
                else
                {
                    notificationService.Show(AppResources.SignIn_Success);
                    MessengerInstance.Send<UserAuthenticatedMessage>(new UserAuthenticatedMessage(isAuthenticated));
                    result = true;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("400") || ex.Message.Contains("401"))
                {
                    // Wierd behavior when credentials are wrong but throws exception '400 Bad Request'
                    notificationService.Show(AppResources.Wrong_Email_Or_Password);
                }
                else
                {
                    notificationService.Show(AppResources.SignIn_Unexpected_Error);
                }
            }
            taskProgressService.FinishProgress();
            return result;
        }
    }
}
