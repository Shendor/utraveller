using Microsoft.Expression.Interactivity.Core;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Control;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTraveller.Service.Model;
using UTraveller.Social.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Auth.ViewModel
{
    public class UserRegistrationViewModel : BaseViewModel
    {
        private static readonly string SOCIAL_AUTH_PAGE = "/Social/SocialAuthBrowserPage.xaml";
        private static readonly string USER_SIGN_IN_PAGE = "/Auth/UserSignInPage.xaml";
        private static readonly string FB_REGISTRATION_TOKEN = "FB_REGISTRATION_TOKEN";

        private NotificationService notificationService;
        private IUserService userService;
        private IFacebookAuthService facebookAuthService;
        private INavigationService navigationService;
        private IFacebookClientService facebookClientService;
        private IImageService imageService;
        private ICancelableTaskProgressService taskProgressService;
        private INetworkConnectionCheckService networkCheckService;
        private ISocialClientAccessToken<string> facebookAccessToken;
        private User userToRegister;

        public UserRegistrationViewModel(IUserService userService, NotificationService notificationService,
            IFacebookAuthService facebookAuthService, INavigationService navigationService, IFacebookClientService facebookClientService,
            IImageService imageService, ICancelableTaskProgressService taskProgressService,
            INetworkConnectionCheckService networkCheckService, ISocialClientAccessToken<string> facebookAccessToken)
        {
            this.userService = userService;
            this.notificationService = notificationService;
            this.facebookAuthService = facebookAuthService;
            this.navigationService = navigationService;
            this.facebookClientService = facebookClientService;
            this.imageService = imageService;
            this.taskProgressService = taskProgressService;
            this.networkCheckService = networkCheckService;
            this.facebookAccessToken = facebookAccessToken;

            RegisterUserCommand = new ActionCommand(RegisterUser);
            SignUpFacebookCommand = new ActionCommand(SignUpFacebook);
            SignInUserCommand = new ActionCommand(SignInUser);
            userToRegister = new User();
        }


        public void Initialize()
        {
            userToRegister = new User();
            MessengerInstance.Unregister<FacebookSignedInMessage>(this, FB_REGISTRATION_TOKEN);
            MessengerInstance.Register<FacebookSignedInMessage>(this, FB_REGISTRATION_TOKEN, OnFacebookSignedIn);
        }


        public override void Cleanup()
        {
            userToRegister = null;
            MessengerInstance.Unregister<FacebookSignedInMessage>(this, FB_REGISTRATION_TOKEN);
        }


        public ICommand RegisterUserCommand
        {
            get;
            private set;
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


        public string Username
        {
            get { return userToRegister.Email; }
            set { userToRegister.Email = value; }
        }


        public string Password
        {
            get;
            set;
        }


        public string RepeatedPassword
        {
            get;
            set;
        }


        public bool IsUserAuthenticated
        {
            get { return userService.GetCurrentUser() != null; }
        }


        private async void RegisterUser()
        {
            if (!networkCheckService.HasConnection)
            {
                notificationService.Show(AppResources.No_Connection_To_SignUp);
            }
            else if (Username == null || Username.Length < 6)
            {
                notificationService.Show(AppResources.SignUp_Incorrect_Email);
            }
            else if (Password == null || Password.Length < 3)
            {
                notificationService.Show(AppResources.SignUp_Incorrect_Password);
            }
            else if (!Password.Equals(RepeatedPassword))
            {
                notificationService.Show(AppResources.SignUp_Password_Not_Matched);
            }
            else if (userService.IsUsernameExists(Username))
            {
                notificationService.Show(AppResources.SignUp_Email_Exist);
            }
            else
            {
                await RegisterUser(Password);
            }
        }

        private async Task RegisterUser(string password)
        {
            taskProgressService.RunIndeterminateProgress(string.Format(AppResources.SignUp_Progress, Username));
            string responseMessage = null;
            bool result = false;
            try
            {
                var taskResult = await userService.RegisterUser(userToRegister, password);
                if (taskProgressService.IsCanceled)
                {
                    return;
                }
                if (taskResult != null)
                {
                    result = taskResult.ResponseObject != null && taskResult.ResponseObject.Value > 0;
                    if (taskResult.Error == null)
                    {
                        responseMessage = AppResources.SignUp_Success;
                    }
                    else
                    {
                        responseMessage = taskResult.Error.Error;
                    }
                }
                else
                {
                    responseMessage = AppResources.SignUp_Fail;
                }
            }
            catch (Exception ex)
            {
                responseMessage = string.Format(AppResources.SignUp_Unexpected_Error, ex.Message);
            }
            finally
            {
                taskProgressService.FinishProgress();
                notificationService.Show(responseMessage);
                MessengerInstance.Send<UserRegisteredMessage>(new UserRegisteredMessage(result));
            }
        }


        private void SignUpFacebook()
        {
            if (!facebookAuthService.IsSignedIn())
            {
                navigationService.Navigate(SOCIAL_AUTH_PAGE, FB_REGISTRATION_TOKEN);
            }
            else
            {
                UpdateUsername();
            }
        }


        private void SignInUser()
        {
            navigationService.Navigate(USER_SIGN_IN_PAGE, FB_REGISTRATION_TOKEN);
        }


        private void OnFacebookSignedIn(FacebookSignedInMessage message)
        {
            if (message.Object)
            {
                UpdateUsername();
            }
        }


        private async void UpdateUsername()
        {
            taskProgressService.RunIndeterminateProgress(AppResources.SignUp_Facebook_Get_Profile_Progress);
            var profile = await facebookClientService.GetUserProfile();
            if (profile != null)
            {
                userToRegister.Email = profile.Email;
                userToRegister.Name = profile.Name;
                userToRegister.AvatarContent = await imageService.ReadRemoteImage(profile.ImageUrl);
                taskProgressService.FinishProgress();
                if (facebookAccessToken.AccessToken != null)
                {
                    var password = profile.Email + DateTime.Now.Millisecond;
                    if (facebookAccessToken.AccessToken.Length > 32 || password.Length > 32)
                    {
                        password = facebookAccessToken.AccessToken.Substring(0, 32);
                    }
                    await RegisterUser(password);
                }
            }
        }
    }
}