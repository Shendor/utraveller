using Microsoft.Expression.Interactivity.Core;
using Microsoft.Live;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Control;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.PhotoCrop.Message;
using UTraveller.Service.Api;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Model;
using UTraveller.Social.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Settings.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        private static readonly string PHOTO_CROP_PAGE = "/PhotoCrop/PhotoCropPage.xaml";
        private static readonly string COLOR_PICKER_PAGE = "/Common/Control/ColorPicker/ColorPalettePickerPage.xaml";
        private static readonly string HELP_PAGE = "/Help/HelpPage.xaml";
        private static readonly string ROUTE_HELP_PAGE = "/Help/RouteHelpPage.xaml";
        private static readonly string BACKGROUND_TOKEN = "background";
        private static readonly string MAIN_COLOR_TOKEN = "mainColor";
        private static readonly string TEXT_COLOR_TOKEN = "textColor";
        private static readonly string CONNECTED = "connected :)";
        private static readonly string DISCONNECTED = "disconnected :(";
        private static readonly string FB_AUTH_TOKEN = "FB_AUTH_FROM_SETTINGS_TOKEN";

        private ICancelableTaskProgressService taskProgressService;
        private INetworkConnectionCheckService networkConnectionCheckService;
        private NotificationService notificationService;
        private INavigationService navigationService;
        private IAppPropertiesService appPropertiesService;
        private IFacebookAuthService facebookAuthService;
        private ISocialClientAccessToken<LiveConnectSession> liveSocialAccessToken;
        private ISocialClientAccessToken<string> facebookAccessToken;
        private IUserService userService;
        private string oneDriveConnectionMessage;
        private string facebookConnectionMessage;
        private string connectedToServerMessage;
        private User currentUser;

        public SettingsViewModel(INavigationService navigationService, IAppPropertiesService appPropertiesService,
            IFacebookAuthService facebookAuthService, IUserService userService,
            ISocialClientAccessToken<LiveConnectSession> liveSocialAccessToken, ISocialClientAccessToken<string> facebookAccessToken,
            ICancelableTaskProgressService taskProgressService, INetworkConnectionCheckService networkConnectionCheckService,
            NotificationService notificationService)
        {
            this.navigationService = navigationService;
            this.appPropertiesService = appPropertiesService;
            this.facebookAuthService = facebookAuthService;
            this.liveSocialAccessToken = liveSocialAccessToken;
            this.facebookAccessToken = facebookAccessToken;
            this.userService = userService;
            this.taskProgressService = taskProgressService;
            this.networkConnectionCheckService = networkConnectionCheckService;
            this.notificationService = notificationService;

            ConnectFacebookCommand = new ActionCommand(ConnectFacebook);
            ConnectToServerCommand = new ActionCommand(ConnectToServer);
            HelpCommand = new ActionCommand(Help);
            WriteFeedbackCommand = new ActionCommand(WiteFeedback);
            OneDriveDetailsCommand = new ActionCommand(OneDriveDetails);
            UpdateAppCommand = new ActionCommand(UpdateApp);

            MessengerInstance.Register<ColorChosenMessage>(this, BACKGROUND_TOKEN, OnBackgroundChanged);
            MessengerInstance.Register<ColorChosenMessage>(this, MAIN_COLOR_TOKEN, OnMainColorChanged);
            MessengerInstance.Register<ColorChosenMessage>(this, TEXT_COLOR_TOKEN, OnTextColorChanged);
            MessengerInstance.Register<SkyDriveSessionChangedMessage>(this, OnSkyDriveSessionChanged);
            MessengerInstance.Register<FacebookSignedInMessage>(this, FB_AUTH_TOKEN, OnFacebookSignedInChanged);
            MessengerInstance.Register<UserAuthenticatedMessage>(this, OnUserAuthenticated);
        }

        private void OnUserAuthenticated(UserAuthenticatedMessage message)
        {
            if (message.Object)
            {
                currentUser = userService.GetCurrentUser();
                App.AppProperties = appPropertiesService.GetPropertiesForUser(currentUser.Id);
                if (AppProperties != null)
                {
                    OnMainColorChanged(new ColorChosenMessage(AppProperties.MainColor));
                    OnBackgroundChanged(new ColorChosenMessage(AppProperties.Background));
                    RefreshColors();
                }
            }
        }


        public void Initialize()
        {
            currentUser = userService.GetCurrentUser();
            OneDriveConnectionMessage = liveSocialAccessToken.AccessToken != null ? CONNECTED : DISCONNECTED;
            FacebookConnectionMessage = facebookAccessToken.AccessToken != null ? CONNECTED : DISCONNECTED;
            ConnectedToServerMessage = currentUser.RESTAccessToken != null ? CONNECTED : DISCONNECTED;
        }


        public override void Cleanup()
        {
        }


        public string Name
        {
            get { return currentUser.Name; }
            set
            {
                currentUser.Name = value;
                userService.UpdateUser(currentUser);
                RaisePropertyChanged("Name");
            }
        }


        public string About
        {
            get { return currentUser.Description; }
            set
            {
                currentUser.Description = value;
                userService.UpdateUser(currentUser);
                RaisePropertyChanged("About");
            }
        }


        public string LiveClientId
        {
            get { return App.LiveClientId; }
        }


        public bool IsConnectToServerAutomatically
        {
            get { return AppProperties.IsConnectToServerAutomatically; }
            set
            {
                AppProperties.IsConnectToServerAutomatically = value;
                UpdateProperties();
                RaisePropertyChanged("IsConnectToServerAutomatically");
            }
        }


        public bool IsAllowGeoPosition
        {
            get { return AppProperties.IsAllowGeoPosition; }
            set
            {
                AppProperties.IsAllowGeoPosition = value;
                UpdateProperties();
                RaisePropertyChanged("IsAllowGeoPosition");
            }
        }


        public bool IsNotConnectedToServer
        {
            get { return currentUser.RESTAccessToken == null; }
        }


        public string ConnectedToServerMessage
        {
            get { return connectedToServerMessage; }
            private set
            {
                connectedToServerMessage = value;
                RaisePropertyChanged("ConnectedToServerMessage");
            }
        }

        public string FacebookConnectionMessage
        {
            get { return facebookConnectionMessage; }
            private set
            {
                facebookConnectionMessage = value;
                RaisePropertyChanged("FacebookConnectionMessage");
            }
        }


        public string OneDriveConnectionMessage
        {
            get { return oneDriveConnectionMessage; }
            private set
            {
                oneDriveConnectionMessage = value;
                RaisePropertyChanged("OneDriveConnectionMessage");
            }
        }


        public double CoverOpacity
        {
            get { return AppProperties.CoverOpacity; }
            set
            {
                AppProperties.CoverOpacity = value;
                UpdateProperties();
                RaisePropertyChanged("CoverOpacity");
            }
        }


        public bool IsLandscapeCover
        {
            get { return AppProperties.IsLandscapeCover; }
            set
            {
                AppProperties.IsLandscapeCover = value;
                UpdateProperties();
                RaisePropertyChanged("IsLandscapeCover");
            }
        }


        public bool IsUploadToFacebook
        {
            get { return AppProperties.IsUploadToFacebook; }
            set
            {
                AppProperties.IsUploadToFacebook = value;
                UpdateProperties();
                RaisePropertyChanged("IsUploadToFacebook");
            }
        }


        public bool IsNotUpToDate
        {
            get { return IsolatedStorageSettings.ApplicationSettings.Contains(App.VERSION); }
        }


        public ICommand ConnectFacebookCommand
        {
            get;
            private set;
        }


        public ICommand ConnectToServerCommand
        {
            get;
            private set;
        }


        public ICommand PrivacyPolicyCommand
        {
            get;
            private set;
        }


        public ICommand HelpCommand
        {
            get;
            private set;
        }


        public ICommand WriteFeedbackCommand
        {
            get;
            private set;
        }


        public ICommand OneDriveDetailsCommand
        {
            get;
            private set;
        }


        public ICommand UpdateAppCommand
        {
            get;
            private set;
        }


        public void ShowChangeAvatarPage(Stream imageStream)
        {
            var message = new ImageToCropChangedMessage(imageStream, 300, 300, UserProfileViewModel.AVATAR_TOKEN);
            MessengerInstance.Send<ImageToCropChangedMessage>(message);
            navigationService.Navigate(PHOTO_CROP_PAGE);
        }


        public void ShowChangeCoverPage(Stream imageStream, int cropWidth, int cropHeight)
        {
            var message = new ImageToCropChangedMessage(imageStream, cropWidth, cropHeight, UserProfileViewModel.COVER_TOKEN);
            MessengerInstance.Send<ImageToCropChangedMessage>(message);
            navigationService.Navigate(PHOTO_CROP_PAGE);
        }


        public void ChangeBackgroundColor()
        {
            MessengerInstance.Send<ColorChangedMessage>(new ColorChangedMessage(BACKGROUND_TOKEN, AppProperties.Background.Color));
            navigationService.Navigate(COLOR_PICKER_PAGE);
        }


        public void ChangeMainColorColor()
        {
            MessengerInstance.Send<ColorChangedMessage>(new ColorChangedMessage(MAIN_COLOR_TOKEN, AppProperties.MainColor.Color));
            navigationService.Navigate(COLOR_PICKER_PAGE);
        }


        public void ChangeTextColorColor()
        {
            MessengerInstance.Send<ColorChangedMessage>(new ColorChangedMessage(TEXT_COLOR_TOKEN, AppProperties.TextColor.Color));
            navigationService.Navigate(COLOR_PICKER_PAGE);
        }


        public bool IsConnectedToFacebook
        {
            get { return facebookAuthService.IsSignedIn(); }
        }


        public Uri GetLogoutUrl()
        {
            return facebookAuthService.GetLogoutUrl();
        }


        private void UpdateProperties()
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser != null)
            {
                appPropertiesService.UpdateProperties(AppProperties, currentUser);
            }
            IsolatedStorageSettings.ApplicationSettings.Save();
        }


        public void ConnectFacebook()
        {
            navigationService.Navigate("/Social/SocialAuthBrowserPage.xaml", FB_AUTH_TOKEN);
        }


        public async void LogoutFacebook()
        {
            var socialData = App.CreateFacebookAppData();
            await facebookAuthService.Logout(socialData);
            FacebookConnectionMessage = DISCONNECTED;
        }


        private async void ConnectToServer()
        {
            if (networkConnectionCheckService.HasConnection)
            {
                taskProgressService.RunIndeterminateProgress("Connecting to Cloud...");
                try
                {
                    await userService.RefreshAccessToken(currentUser);
                    ConnectedToServerMessage = currentUser.RESTAccessToken != null ? CONNECTED : DISCONNECTED;
                }
                catch (WebServiceException)
                {
                    notificationService.Show("Sorry, we've got an unexpected error from Cloud :( Please, try again later.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Cannot connect to server: " + ex.Message);
                }
                finally
                {
                    taskProgressService.FinishProgress();
                }
            }
            else
            {
                notificationService.Show("You dont have Internet connection :(");
            }
        }


        private void Help()
        {
            navigationService.Navigate(HELP_PAGE);
        }


        private void WiteFeedback()
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();

            emailComposeTask.Subject = "Support";
            emailComposeTask.To = "utraveler_helpdesk@yahoo.com";

            emailComposeTask.Show();
        }


        private void OneDriveDetails()
        {
            navigationService.Navigate(ROUTE_HELP_PAGE);
        }


        private void UpdateApp()
        {
            MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
            marketplaceDetailTask.ContentIdentifier = Windows.ApplicationModel.Store.CurrentApp.AppId.ToString();
            marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
            marketplaceDetailTask.Show();
        }


        private void OnTextColorChanged(ColorChosenMessage message)
        {
            AppProperties.TextColor = message.Object;
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(TEXT_COLOR_KEY))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(TEXT_COLOR_KEY, message.Object.Color.ToString());
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings[TEXT_COLOR_KEY] = message.Object.Color.ToString();
            }
            RaisePropertyChanged("TextColor");
            UpdateProperties();
        }


        private void OnMainColorChanged(ColorChosenMessage message)
        {
            AppProperties.MainColor = message.Object;
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(MAIN_COLOR_KEY))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(MAIN_COLOR_KEY, message.Object.Color.ToString());
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings[MAIN_COLOR_KEY] = message.Object.Color.ToString();
            }
            RaisePropertyChanged("MainColor");
            UpdateProperties();
        }


        private void OnBackgroundChanged(ColorChosenMessage message)
        {
            AppProperties.Background = message.Object;
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(BACKGROUND_COLOR_KEY))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(BACKGROUND_COLOR_KEY, message.Object.Color.ToString());
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings[BACKGROUND_COLOR_KEY] = message.Object.Color.ToString();
            }
            RaisePropertyChanged("BackgroundColor");
            UpdateProperties();
        }

       
        protected void OnSkyDriveSessionChanged(SkyDriveSessionChangedMessage message)
        {
            liveSocialAccessToken.AccessToken = message.Object;
            OneDriveConnectionMessage = liveSocialAccessToken.AccessToken != null ? CONNECTED : DISCONNECTED;
        }


        private void OnFacebookSignedInChanged(FacebookSignedInMessage message)
        {
            FacebookConnectionMessage = message.Object ? CONNECTED : DISCONNECTED;
        }

    }
}
