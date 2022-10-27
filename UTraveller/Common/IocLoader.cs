using GalaSoft.MvvmLight.Messaging;
using LocalRepositoryImpl.UTraveller.LocalRepository;
using LocalRepositoryImpl.UTraveller.LocalRepository.Implementation;
using Ninject.Modules;
using RepositoryApi.UTraveller.Repository.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Database;
using UTraveller.EventDetails.ViewModel;
using UTraveller.EventList.ViewModel;
using UTraveller.EventMap.ViewModel;
using UTraveller.Service.Api;
using UTraveller.Service.Implementation;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Mapper;
using UTravellerModel.UTraveller.Model;
using Ninject;
using UTraveller.ImageChooser.ViewModel;
using UTraveller.PhotoViewer.ViewModel;
using UTraveller.SkyDrive.ViewModel;
using Microsoft.Live;
using UTraveller.Service.Model;
using UTraveller.Routes.ViewModel;
using UTraveller.MoneySpendings.ViewModel;
using UTraveller.PhotoCrop.ViewModel;
using UTraveller.MessagePost.ViewModel;
using UTraveller.Common.Control;
using UTraveller.Social.ViewModel;
using UTraveller.Common.Control.ProgressBar;
using UTraveller.Auth.ViewModel;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Implementation.Remote;
using UTravellerModel.UTraveller.Model.Remote;
using ServiceImpl.UTraveller.Service.Model;
using UTraveller.Settings.ViewModel;
using UTraveller.Sync.ViewModel;
using UTraveller.Service.Implementation.PhotoTransfer;
using UTraveller.Common.Control.Dialog;
using UTraveller.Walkthrough.ViewModel;
using UTraveller.Help.ViewModel;
using UTraveller.TripPlanEditor.ViewModel;
using UTraveller.Common.Control.DateRangeEditor;

namespace UTraveller.Common
{
    public class IocLoader : NinjectModule
    {
        public override void Load()
        {
            Bind<LocalDatabase>().To<UTravellerLocalDatabase>().InSingletonScope();

            Bind<IModelMapper<User, UserEntity>>().To<UserMapper>().InSingletonScope();
            Bind<IModelMapper<AppProperties, AppPropertiesEntity>>().To<AppPropertiesMapper>().InSingletonScope();
            Bind<IModelMapper<Photo, PhotoEntity>>().To<PhotoModelMapper>().InSingletonScope();
            Bind<IModelMapper<PhotoPushpin, PushpinEntity>>().To<PushpinModelMapper>().InSingletonScope();
            Bind<IModelMapper<Event, EventEntity>>().To<EventModelMapper>().InSingletonScope();
            Bind<IModelMapper<Route, RouteEntity>>().To<RouteMapper>().InSingletonScope();
            Bind<IModelMapper<RoutePushpin, RoutePushpinEntity>>().To<RoutePushpinMapper>().InSingletonScope();
            Bind<IModelMapper<MoneySpending, MoneySpendingEntity>>().To<MoneySpendingMapper>().InSingletonScope();
            Bind<IModelMapper<UTravellerModel.UTraveller.Model.Message, MessageEntity>>().To<MessageMapper>().InSingletonScope();
            Bind<IModelMapper<User, UserRemoteModel>>().To<UserRemoteModelMapper>().InSingletonScope();
            Bind<IModelMapper<User, BaseUserRemoteModel>>().To<BaseUserRemoteModelMapper>().InSingletonScope();
            Bind<IModelMapper<AppProperties, UserSettingRemoteModel>>().To<UserSettingRemoteModelMapper>().InSingletonScope();
            Bind<IModelMapper<TripPlan, TripPlanEntity>>().To<TripPlanMapper>().InSingletonScope();

            Bind<TestLoaderRepository>().To<TestLoaderRepository>().InSingletonScope();
            Bind<IUserRepository>().To<UserRepository>().InSingletonScope();
            Bind<IAppPropertiesRepository>().To<AppPropertiesRepository>().InSingletonScope();
            Bind<IEventRepository>().To<EventRepository>().InSingletonScope();
            Bind<IPhotoRepository>().To<PhotoRepository>().InSingletonScope();
            Bind<IRouteRepository>().To<RouteRepository>().InSingletonScope();
            Bind<IRoutePushpinRepository>().To<RoutePushpinRepository>().InSingletonScope();
            Bind<IMoneySpendingRepository>().To<MoneySpendingRepository>().InSingletonScope();
            Bind<IMessageRepository>().To<MessageRepository>().InSingletonScope();
            Bind<ITripPlanRepository>().To<TripPlanRepository>().InSingletonScope();

            Bind<IUserService>().To<LocalUserService>().InSingletonScope();
            Bind<IAppPropertiesService>().To<AppPropertiesService>().InSingletonScope();
            Bind<IEventService>().To<EventService>().InSingletonScope();
            Bind<IPhotoService>().To<PhotoService>().InSingletonScope();
            Bind<IRouteService>().To<RouteService>().InSingletonScope();
            Bind<IExpenseService>().To<ExpenseService>().InSingletonScope();
            Bind<IMessageService>().To<MessageService>().InSingletonScope();
            Bind<IGeoCoordinateService>().To<GeoCoordinateService>().InSingletonScope();
            Bind<ISocialClientAccessToken<string>>().To<FacebookSocialClient>().InSingletonScope();
            Bind<ISocialClientAccessToken<LiveConnectSession>>().To<LiveSocialClient>().InSingletonScope();
            Bind<IFacebookAuthService>().To<FacebookAuthService>().InSingletonScope();
            Bind<IMicrosoftLiveAuthService>().To<MicrosoftLiveAuthService>().InSingletonScope();
            Bind<IFacebookClientService>().To<FacebookClientService>().InSingletonScope();
            Bind<INetworkConnectionCheckService>().To<NetworkConnectionCheckService>().InSingletonScope();
            Bind<IWebService>().To<UTravelerWebService>().InSingletonScope();
            //Bind<IPhotoUploadService<OneDrivePhotoUploadRequest>>().To<PhotoUploadOneDriveService>().InSingletonScope();
            Bind<IPhotoUploadService<FacebookPhotoUploadRequest>>().To<PhotoUploadFacebookService>().InSingletonScope();
            Bind<IZipExtractService>().To<ZipExtractService>().InSingletonScope();
            Bind<ILiveAuthService>().To<LiveAuthService>().InSingletonScope();
            Bind<ITripPlanService>().To<TripPlanService>().InSingletonScope();

            Bind<INavigationService>().To<NavigationService>();
            Bind<IPhotoUploader>().To<PhotoUploader>();
            Bind<IImageCompressionService>().To<ImageCompressionService>().InSingletonScope();
            Bind<IImageLoaderService>().To<RemotePhotoLoaderService>().When(a => a.Target == null || a.Target.Name == "remotePhotoLoaderService").InSingletonScope().Named("remotePhotoLoaderService");
            Bind<IImageLoaderService>().To<MediaLibraryPhotoLoaderService>().When(a => a.Target == null || a.Target.Name == "mediaLibraryPhotoLoaderService").InSingletonScope().Named("mediaLibraryPhotoLoaderService");
            Bind<IImageInitializer>().To<MediaLibraryPhotoImageInitializer>().InSingletonScope().Named("mediaLibraryPhotoImageInitializer");
            Bind<IParameterContainer<string>>().To<ParameterContainer>().InSingletonScope();
            Bind<IFileExplorerService<OneDriveFileRequest>>().To<OneDriveExplorerService>().InSingletonScope();
            Bind<IRouteFileReader>().To<RouteFileReader>().InSingletonScope();
            Bind<IImageCropService>().To<ImageCropService>().InSingletonScope();
            Bind<IImageService>().To<ImageService>().InSingletonScope();
            Bind<ICancelableTaskProgressService>().To<TaskProgressService>().InSingletonScope();
            Bind<ITaskProgressService>().To<BackgroundTaskProgressService>().InSingletonScope().Named("backgroundTaskProgressService");
            Bind<ITaskExecutionManager>().To<TaskExecutionManager>().InSingletonScope();
            Bind<ITripBackupService>().To<TripBackupService>().InSingletonScope();
            

            Bind<EventDetailsViewModel>().To<EventDetailsViewModel>().InSingletonScope();
            Bind<EventMapViewModel>().To<EventMapViewModel>().InSingletonScope();
            Bind<EventViewModel>().To<EventViewModel>().InSingletonScope();
            Bind<EventPhotoListChooserViewModel>().To<EventPhotoListChooserViewModel>().InSingletonScope();
            Bind<PhonePhotoGroupedListChooserViewModel>().To<PhonePhotoGroupedListChooserViewModel>().InSingletonScope();
            Bind<DetailedImageChooserViewModel>().To<DetailedImageChooserViewModel>().InSingletonScope();
            Bind<PhotoViewerViewModel>().To<PhotoViewerViewModel>().InSingletonScope();
            Bind<RoutesViewModel>().To<RoutesViewModel>().InSingletonScope();
            Bind<RouteSkyDriveExplorerViewModel>().To<RouteSkyDriveExplorerViewModel>().InSingletonScope();
            Bind<MoneySpendingViewModel>().To<MoneySpendingViewModel>().InSingletonScope();
            Bind<UserProfileViewModel>().To<UserProfileViewModel>().InSingletonScope();
            Bind<PhotoCropViewModel>().To<PhotoCropViewModel>().InSingletonScope();
            Bind<MessagePostViewModel>().To<MessagePostViewModel>().InSingletonScope();
            Bind<ColorPickerViewModel>().To<ColorPickerViewModel>().InSingletonScope();
            Bind<SocialAuthViewModel>().To<SocialAuthViewModel>().InSingletonScope();
            Bind<NotificationService>().To<NotificationService>().InSingletonScope();
            Bind<PushpinDescriptionService>().To<PushpinDescriptionService>().InSingletonScope();
            Bind<ConfirmationService>().To<ConfirmationService>().InSingletonScope();
            Bind<NotificationWindow>().To<NotificationWindow>().InSingletonScope();
            Bind<PushpinDescriptionDialog>().To<PushpinDescriptionDialog>().InSingletonScope();
            Bind<ConfirmationDialog>().To<ConfirmationDialog>().InSingletonScope();
            Bind<ProgressBarDialog>().To<ProgressBarDialog>().InSingletonScope();
            Bind<BackgroundProgressBar>().To<BackgroundProgressBar>().InSingletonScope();
            Bind<UserRegistrationViewModel>().To<UserRegistrationViewModel>().InSingletonScope();
            Bind<SettingsViewModel>().To<SettingsViewModel>().InSingletonScope();
            Bind<MessagesChooserViewModel>().To<MessagesChooserViewModel>().InSingletonScope();
            Bind<WalkthroughViewModel>().To<WalkthroughViewModel>().InSingletonScope();
            Bind<HelpViewModel>().To<HelpViewModel>().InSingletonScope();
            Bind<TripPlanViewModel>().To<TripPlanViewModel>().InSingletonScope();
            Bind<EditPlanItemViewModel>().To<EditPlanItemViewModel>().InSingletonScope();
            Bind<EditPlanItemCoordinateViewModel>().To<EditPlanItemCoordinateViewModel>().InSingletonScope();
            Bind<DateRangeEditorViewModel>().To<DateRangeEditorViewModel>().InSingletonScope();

            //InitializeRemoteImageLoaderService();
            RegisterSyncService();
            InitializeViewModels();
        }

        //private void InitializeRemoteImageLoaderService()
        //{
        //    var mediaLibraryInitializerService = (MediaLibraryPhotoImageInitializer)Kernel.Get<IImageInitializer>("mediaLibraryPhotoImageInitializer");
        //    mediaLibraryInitializerService.MediaLibraryPhotoLoaderService = Kernel.Get<IImageLoaderService>("mediaLibraryPhotoLoaderService");

        //    var remoteLoaderService = (RemotePhotoLoaderService)Kernel.Get<IImageLoaderService>("remotePhotoLoaderService");
        //    remoteLoaderService.MediaLibraryImageLoaderService = Kernel.Get<IImageLoaderService>("mediaLibraryPhotoLoaderService");
        //}

        private void RegisterSyncService()
        {
            Bind<ISyncService>().To<SyncService>().InSingletonScope();
            SyncService syncService = (SyncService)Kernel.Get<ISyncService>();
        }

        private void InitializeViewModels()
        {
            Kernel.Get<EventViewModel>();
            Kernel.Get<EventDetailsViewModel>();
            Kernel.Get<EventMapViewModel>();
            Kernel.Get<EventPhotoListChooserViewModel>();
            Kernel.Get<PhonePhotoGroupedListChooserViewModel>();
            Kernel.Get<DetailedImageChooserViewModel>();
            Kernel.Get<PhotoViewerViewModel>();
            Kernel.Get<RouteSkyDriveExplorerViewModel>();
            Kernel.Get<RoutesViewModel>();
            Kernel.Get<MoneySpendingViewModel>();
            Kernel.Get<PhotoCropViewModel>();
            Kernel.Get<MessagePostViewModel>();
            Kernel.Get<ColorPickerViewModel>();
            Kernel.Get<SocialAuthViewModel>();
            Kernel.Get<PushpinDescriptionService>();
            Kernel.Get<NotificationService>();
            Kernel.Get<NotificationWindow>();
            Kernel.Get<PushpinDescriptionDialog>();
            Kernel.Get<ConfirmationDialog>();
            Kernel.Get<ProgressBarDialog>();
            Kernel.Get<BackgroundProgressBar>();
            Kernel.Get<UserRegistrationViewModel>();
            Kernel.Get<SettingsViewModel>();
            Kernel.Get<MessagesChooserViewModel>();
            Kernel.Get<WalkthroughViewModel>();
            Kernel.Get<TripPlanViewModel>();
            Kernel.Get<EditPlanItemViewModel>();
            Kernel.Get<EditPlanItemCoordinateViewModel>();
            Kernel.Get<DateRangeEditorViewModel>();
        }
    }
}
