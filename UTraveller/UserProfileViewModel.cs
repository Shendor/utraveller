using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Live;
using Microsoft.Phone.Tasks;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UTraveller.Common.Control;
using UTraveller.Common.Converter;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.EventList.ViewModel;
using UTraveller.PhotoCrop.Message;
using UTraveller.Service.Api;
using UTraveller.Service.Model;
using UTraveller.Social.Messages;
using UTravellerModel.UTraveller.Model;
using Windows.ApplicationModel;

namespace UTraveller
{
    public class UserProfileViewModel : BaseViewModel
    {
        private static readonly string PHOTO_CROP_PAGE = "/PhotoCrop/PhotoCropPage.xaml";
        private static readonly string SETTINGS_PAGE = "/Settings/SettingsPage.xaml";
        private static readonly string USER_REGISTRATION_PAGE = "/Auth/UserRegistrationPage.xaml";
        private static readonly string USER_SIGN_IN_PAGE = "/Auth/UserSignInPage.xaml";
        private static readonly string WALKTHROUGH_PAGE = "/Walkthrough/WalkthroughPage.xaml";
        public static readonly string AVATAR_TOKEN = "avatar";
        public static readonly string COVER_TOKEN = "cover";

        private INavigationService navigationService;
        private IUserService userService;
        private IEventService eventService;
        private IPhotoService photoService;
        private IImageCropService imageCropService;
        private IAppPropertiesService appPropertiesService;
        private IFacebookAuthService facebookAuthService;
        private ITaskProgressService backgroundTaskProgressService;
        private NotificationService notificationService;
        private EventViewModel eventViewModel;
        private User currentUser;

        public UserProfileViewModel(INavigationService navigationService, IUserService userService,
            IImageCropService imageCropService, EventViewModel eventViewModel, IAppPropertiesService appPropertiesService,
            IFacebookAuthService facebookAuthService, IPhotoService photoService, IEventService eventService,
            NotificationService notificationService,
            [Named("backgroundTaskProgressService")] ITaskProgressService backgroundTaskProgressService)
        {
            this.navigationService = navigationService;
            this.userService = userService;
            this.imageCropService = imageCropService;
            this.eventViewModel = eventViewModel;
            this.appPropertiesService = appPropertiesService;
            this.facebookAuthService = facebookAuthService;
            this.photoService = photoService;
            this.eventService = eventService;
            this.notificationService = notificationService;
            this.backgroundTaskProgressService = backgroundTaskProgressService;

            Cover = new BitmapImage();
            Cover.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

            Avatar = new BitmapImage();
            Avatar.CreateOptions = BitmapCreateOptions.IgnoreImageCache;

            RegisterUserCommand = new ActionCommand(RegisterUser);
            SignInUserCommand = new ActionCommand(SignInUser);
            DeleteAvatarCommand = new ActionCommand(DeleteAvatar);
            DeleteCoverCommand = new ActionCommand(DeleteCover);
            SettingsCommand = new ActionCommand(ModifySettings);

            MessengerInstance.Register<ImageCroppedMessage>(this, COVER_TOKEN, OnCoverImageCropped);
            MessengerInstance.Register<ImageCroppedMessage>(this, AVATAR_TOKEN, OnAvatarImageCropped);
            MessengerInstance.Register<UserRegisteredMessage>(this, OnUserRegistered);
        }


        public void Initialize()
        {
            if ((currentUser = userService.GetCurrentUser()) != null)
            {
                backgroundTaskProgressService.RunIndeterminateProgress();

                App.AppProperties = appPropertiesService.GetPropertiesForUser(currentUser.Id);
                
                eventViewModel.CurrentUser = currentUser;
                eventViewModel.Initialize();
                LoadAvatarAndCover();

                EventsQuantity = eventService.GetEventsQuantity(currentUser);
                PhotosQuantity = photoService.GetPhotosQuantity(currentUser.Id);

                facebookAuthService.TrySignIn(App.CreateFacebookAppData());

                RaisePropertyChanged("EventsQuantity");
                RaisePropertyChanged("PhotosQuantity");
                RaisePropertyChanged("Name");
                RaisePropertyChanged("About");
                RaisePropertyChanged("CoverOpacity");
                RaisePropertyChanged("HasNoEvents");
                RefreshColors();

                backgroundTaskProgressService.FinishProgress();
            }
        }


        public override void Cleanup()
        {
            eventViewModel.Cleanup();
        }


        public void TryOpenCurrentEvent()
        {
            eventViewModel.OpenCurrentEvent();
        }


        #region Commands

        public ICommand SettingsCommand
        {
            get;
            private set;
        }

        public ICommand RegisterUserCommand
        {
            get;
            private set;
        }


        public ICommand SignInUserCommand
        {
            get;
            private set;
        }


        public ICommand DeleteAvatarCommand
        {
            get;
            private set;
        }


        public ICommand DeleteCoverCommand
        {
            get;
            private set;
        }

        #endregion

        #region Properties

        public string LiveClientId
        {
            get { return App.LiveClientId; }
        }


        public EventViewModel EventViewModel
        {
            get { return eventViewModel; }
        }


        public string Name
        {
            get
            {
                return (currentUser != null) ? currentUser.Name : null;
            }
            set
            {
                if (currentUser != null)
                {
                    currentUser.Name = value;
                    userService.UpdateUser(currentUser);
                }
            }
        }


        public string About
        {
            get { return (currentUser != null) ? currentUser.Description : ""; }
            set
            {
                if (currentUser != null)
                {
                    currentUser.Description = value;
                    userService.UpdateUser(currentUser);
                }
            }
        }


        public double CoverOpacity
        {
            get { return AppProperties.CoverOpacity; }
        }


        public BitmapImage Avatar
        {
            get;
            private set;
        }


        public BitmapImage Cover
        {
            get;
            private set;
        }


        public int EventsQuantity
        {
            get;
            private set;
        }


        public int PhotosQuantity
        {
            get;
            private set;
        }

        #endregion

        public void ShowChangeAvatarPage(Stream imageStream)
        {
            var message = new ImageToCropChangedMessage(imageStream, 300, 300, AVATAR_TOKEN);
            MessengerInstance.Send<ImageToCropChangedMessage>(message);
            navigationService.Navigate(PHOTO_CROP_PAGE);
        }


        public void ShowChangeCoverPage(Stream imageStream, int cropWidth, int cropHeight)
        {
            var message = new ImageToCropChangedMessage(imageStream, cropWidth, cropHeight, COVER_TOKEN);
            MessengerInstance.Send<ImageToCropChangedMessage>(message);
            navigationService.Navigate(PHOTO_CROP_PAGE);
        }


        public async Task<bool> TryAuthenticateCurrentUser()
        {
            backgroundTaskProgressService.RunIndeterminateProgress();
            currentUser = await userService.TryAuthenticateCurrentUser();
            if (currentUser == null)
            {
                backgroundTaskProgressService.FinishProgress();
            }
            return currentUser != null;
        }


        private void LoadAvatarAndCover()
        {
            if (currentUser != null)
            {
                if (currentUser.CoverContent != null)
                {
                    BytesToBitmapImageConverter.ToStream(Cover, currentUser.CoverContent);
                    RaisePropertyChanged("Cover");
                }
                else
                {
                    DeleteCover();
                }
                if (currentUser.AvatarContent != null)
                {
                    BytesToBitmapImageConverter.ToStream(Avatar, currentUser.AvatarContent);
                    RaisePropertyChanged("Avatar");
                }
                else
                {
                    DeleteAvatar();
                }
            }
        }


        public async void CheckIfVersionIsUpToDate()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(App.VERSION) &&
                !await appPropertiesService.IsVersionUpToDate(App.Version))
            {
                IsolatedStorageSettings.ApplicationSettings.Add(App.VERSION, App.Version);
                IsolatedStorageSettings.ApplicationSettings.Save();

                notificationService.Show("New version is available in Store. Please, go to 'Settings' and in 'about' section tap on Update button.");
            }
            else if (IsolatedStorageSettings.ApplicationSettings.Contains(App.VERSION))
            {
                var version = IsolatedStorageSettings.ApplicationSettings[App.VERSION];
                if (!App.Version.Equals(version))
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove(App.VERSION);
                    IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
        }


        #region Message handlers

        private void OnUserRegistered(UserRegisteredMessage message)
        {
            if (message.Object)
            {
                Initialize();
            }
        }


        private void OnCoverImageCropped(ImageCroppedMessage message)
        {
            currentUser = userService.GetCurrentUser();
            if (message.Object != null && currentUser != null)
            {
                using (MemoryStream coverCompressedImage = new MemoryStream())
                {
                    imageCropService.ChangeResolution(message.Object, coverCompressedImage, 800, 520);
                    currentUser.CoverContent = coverCompressedImage.ToArray();
                    userService.UpdateUser(currentUser);
                    Cover.SetSource(coverCompressedImage);
                }
            }
        }


        private void OnAvatarImageCropped(ImageCroppedMessage message)
        {
            if (message.Object != null)
            {
                using (MemoryStream avatarCompressedImage = new MemoryStream())
                {
                    imageCropService.ChangeResolution(message.Object, avatarCompressedImage, 300, 300);
                    currentUser.AvatarContent = avatarCompressedImage.ToArray();
                    userService.UpdateUser(currentUser);
                    Avatar.SetSource(avatarCompressedImage);
                }
            }
        }

        #endregion

        #region Command handlers

        public void ShowWalkthrough()
        {
            navigationService.Navigate(WALKTHROUGH_PAGE, "true");
        }


        private void RegisterUser()
        {
            navigationService.Navigate(USER_REGISTRATION_PAGE);
        }


        private void SignInUser()
        {
            navigationService.Navigate(USER_SIGN_IN_PAGE);
        }


        private void DeleteAvatar()
        {
            currentUser.AvatarContent = null;
            userService.UpdateUser(currentUser);
            Avatar.ClearValue(BitmapImage.UriSourceProperty);
            RaisePropertyChanged("Avatar");
        }


        private void DeleteCover()
        {
            currentUser.CoverContent = null;
            userService.UpdateUser(currentUser);
            Cover.ClearValue(BitmapImage.UriSourceProperty);
            RaisePropertyChanged("Cover");
        }


        private void ModifySettings()
        {
            navigationService.Navigate(SETTINGS_PAGE);
        }
        #endregion
    }
}