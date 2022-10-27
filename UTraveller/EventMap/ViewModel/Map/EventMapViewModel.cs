using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UTraveller.Common.Control;
using UTraveller.Common.Control.Dialog;
using UTraveller.Common.Message;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTraveller.EventDetails.ViewModel;
using UTraveller.EventMap.Messages;
using UTraveller.EventMap.ViewModel.Map;
using UTraveller.ImageChooser.Messages;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTraveller.TripPlanEditor.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel
{
    public class EventMapViewModel : BaseViewModel
    {
        private static readonly string PHOTO_VIEWER_PATH = "/PhotoViewer/PhotoViewerPage.xaml";
        private static readonly string PHOTO_LIST_CHOOSER = "/ImageChooser/Control/PhotoListChooser.xaml";
        private static readonly string MESSAGE_LIST_CHOOSER = "/ImageChooser/Control/MessagesChooser.xaml";
        private static readonly string EDIT_PLAN_ITEM_PATH = "/TripPlanEditor/EditPlanItemPage.xaml";
        private static readonly string ROUTES_LIST = "/Routes/RoutesPage.xaml";
        private static readonly string MAP_HELP = "/Help/MapHelpPage.xaml";

        private ICollection<BasePushpinItemInMapViewModel<IPushpinItem>> pushpinItems;
        private ICollection<TimeLineItemPushpinViewModel> pushpins;
        private ICollection<RoutePushpinViewModel> routePushpins;
        private IParameterContainer<string> parameterContainer;
        private IPhotoService photoService;
        private IMessageService messageService;
        private IRouteService routeService;
        private INavigationService navigationService;
        private IGeoCoordinateService geoCoordinateService;
        private ConfirmationService confirmationService;
        private NotificationService notificationService;
        private PushpinDescriptionService routePushpinDescriptionService;
        private Event currentEvent;
        private TimeLineItemPushpinViewModel selectedPushpin;
        private GeoCoordinateWatcher currentLocationWatcher;
        private IUserService userService;
        private IAppPropertiesService appPropertiesService;
        private bool isPhotoPushpinsVisible;
        private bool isPlanItemPushpinsVisible;
        private bool isRoutesVisible;
        private PlanItemsPushpinsViewModel planItemsPushpinsViewModel;

        public EventMapViewModel(IParameterContainer<string> parameterContainer,
            INavigationService navigationService, IPhotoService photoService, IMessageService messageService,
            IRouteService routeService, IGeoCoordinateService geoCoordinateService,
            NotificationService notificationService, ConfirmationService confirmationService, IUserService userService,
            IAppPropertiesService appPropertiesService, PushpinDescriptionService routePushpinDescriptionService)
        {
            this.parameterContainer = parameterContainer;
            this.photoService = photoService;
            this.messageService = messageService;
            this.routeService = routeService;
            this.navigationService = navigationService;
            this.geoCoordinateService = geoCoordinateService;
            this.notificationService = notificationService;
            this.confirmationService = confirmationService;
            this.userService = userService;
            this.appPropertiesService = appPropertiesService;
            this.routePushpinDescriptionService = routePushpinDescriptionService;

            pushpins = new ObservableCollection<TimeLineItemPushpinViewModel>();
            routePushpins = new ObservableCollection<RoutePushpinViewModel>();
            pushpinItems = new ObservableCollection<BasePushpinItemInMapViewModel<IPushpinItem>>();
            planItemsPushpinsViewModel = new PlanItemsPushpinsViewModel();

            isPhotoPushpinsVisible = true;
            isPlanItemPushpinsVisible = true;
            isRoutesVisible = true;

            MessengerInstance.Register<MessageDeletedMessage>(this, OnMessageDeleted);
            MessengerInstance.Register<DeletedEventPhotoMessage>(this, OnPhotoDeleted);
            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);
            MessengerInstance.Register<ShowPushpinItemsOnMapMessage>(this, OnPhotosChanged);
            MessengerInstance.Register<ShowPlanItemPushpinsOnMapMessage>(this, OnShowPlanItems);
            MessengerInstance.Register<PhotoPushpinDeletedMessage>(this, OnPhotoPushpinDeleted);
            MessengerInstance.Register<FindPushpinItemInPushpinMessage>(this, OnFindPushpinItemInPushpin);
            MessengerInstance.Register<ViewPushpinPhotosMessage>(this, OnViewPushpinTimeLineItems);
            MessengerInstance.Register<AddPhotosToPushpinMessage>(this, OnAddPhotosToPushpin);
            MessengerInstance.Register<AddMessagesToPushpinMessage>(this, OnAddMessagesToPushpin);
            MessengerInstance.Register<EventPhotosChosenMessage>(this, GetType().ToString(), OnPhotosAddedToPushpin);
            MessengerInstance.Register<MessagesChosenMessage>(this, OnMessagesAddedToPushpin);
            MessengerInstance.Register<RouteChosenMessage>(this, OnRouteChosen);
            MessengerInstance.Register<PhotoDeletedFromMapMessage>(this, OnPhotoDeletedFromMap);
            MessengerInstance.Register<DeleteMessageFromMapMessage>(this, OnMessageDeletedFromMap);
            MessengerInstance.Register<LaunchCreatePlanItemFromRoutePushpinPageMessage>(this, OnLaunchCreatePlanItemFromRoutePushpinPage);
            MessengerInstance.Register<PlanItemDeletedMessage>(this, OnPlanItemDeleted);
        }


        public void Initialize()
        {
            MessengerInstance.Register<PlanItemSavedMessage>(this, OnPlanItemSaved);
            UpdatePhotoPushpins();
            InitializePushpins();
        }


        public override void Cleanup()
        {
            MessengerInstance.Unregister<PlanItemSavedMessage>(this);
            PushpinItems.Clear();
            Pushpins.Clear();
            RoutePushpins.Clear();
            planItemsPushpinsViewModel.ClearPushpins();
            StopLocationWatcher();
        }


        public ICollection<BasePushpinItemInMapViewModel<IPushpinItem>> PushpinItems
        {
            get { return pushpinItems; }
        }


        public ICollection<TimeLineItemPushpinViewModel> Pushpins
        {
            get { return pushpins; }
        }


        public ICollection<RoutePushpinViewModel> RoutePushpins
        {
            get { return routePushpins; }
        }


        public ICollection<PlanItemPushpinViewModel> PlanItemPushpins
        {
            get { return planItemsPushpinsViewModel.PlanItemPushpins; }
        }


        public ICollection<PlanItemLegendViewModel> PlanItemsLegend
        {
            get { return planItemsPushpinsViewModel.PlanItemsLegend; }
        }


        public bool IsPhotoPushpinsVisible
        {
            get { return isPhotoPushpinsVisible; }
            set
            {
                isPhotoPushpinsVisible = value;
                UpdatePushpinsVisibility(Pushpins, isPhotoPushpinsVisible);
            }
        }


        public bool IsPlanItemPushpinsVisible
        {
            get { return isPlanItemPushpinsVisible; }
            set
            {
                isPlanItemPushpinsVisible = value;
                UpdatePushpinsVisibility(PlanItemPushpins, isPlanItemPushpinsVisible);
            }
        }


        public bool IsRoutesVisible
        {
            get { return isRoutesVisible; }
            set
            {
                isRoutesVisible = value;
                UpdatePushpinsVisibility(RoutePushpins, isRoutesVisible);
            }
        }


        public ICollection<GeoCoordinate> GetPushpinsCoordinates()
        {
            var coordinates = new HashSet<GeoCoordinate>();
            if (IsPhotoPushpinsVisible)
            {
                foreach (var pushpin in Pushpins)
                {
                    coordinates.Add(pushpin.Coordinate);
                }
            }
            if (IsRoutesVisible)
            {
                foreach (var pushpin in RoutePushpins)
                {
                    coordinates.Add(pushpin.Pushpin.Coordinate);
                }
            }
            if (IsPlanItemPushpinsVisible)
            {
                foreach (var pushpin in PlanItemPushpins)
                {
                    if (!pushpin.BasePlanItem.IsVisited)
                    {
                        coordinates.Add(pushpin.BasePlanItem.Coordinate);
                    }
                }
            }
            return coordinates;
        }


        public void ShowRoutesList()
        {
            navigationService.Navigate(ROUTES_LIST);
        }


        public void ShowMapHelp()
        {
            navigationService.Navigate(MAP_HELP);
        }


        public void AddPushpin(GeoCoordinate coordinate)
        {
            Pushpins.Add(new TimeLineItemPushpinViewModel(coordinate));
        }


        public void ShowPushpinDescription(string description, GeoCoordinate coordinate)
        {
            routePushpinDescriptionService.Show(description, coordinate);
        }


        public void ShowMyLocation(EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>> positionChangedHandler)
        {
            var properties = appPropertiesService.GetPropertiesForUser(userService.GetCurrentUser().Id);
            if (properties.IsAllowGeoPosition)
            {
                if (currentLocationWatcher == null)
                {
                    currentLocationWatcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default)
                    {
                        MovementThreshold = 20
                    };
                    currentLocationWatcher.PositionChanged += positionChangedHandler;
                }
                currentLocationWatcher.Start();
            }
            else
            {
                notificationService.Show("Sorry, but you don't allow us to show your position. Please, go to 'Settings' to switch it on.");
            }
        }


        private void InitializePushpins()
        {
            if (currentEvent != null)
            {
                Pushpins.Clear();
                var pushpins = new Dictionary<GeoCoordinate, TimeLineItemPushpinViewModel>();
                foreach (var photoThumbnail in PushpinItems)
                {
                    var photo = photoThumbnail.DateItem;
                    if (photo.Coordinate != null)
                    {
                        GeoCoordinate key = null;
                        foreach (var pushpin in pushpins)
                        {
                            if (geoCoordinateService.IsNeighbours(pushpin.Key, photo.Coordinate))
                            {
                                key = pushpin.Key;
                                break;
                            }
                        }
                        if (key == null)
                        {
                            key = photo.Coordinate;
                            pushpins.Add(key, new TimeLineItemPushpinViewModel(key)
                            {
                                Visibility = IsPhotoPushpinsVisible ? Visibility.Visible : Visibility.Collapsed
                            });
                        }
                        pushpins[key].TimeLineItems.Add(photoThumbnail.DateItem);
                    }
                }

                foreach (var pushpin in pushpins)
                {
                    Pushpins.Add(pushpin.Value);
                }
            }
        }


        private void UpdatePhotoPushpins()
        {
            var visibility = IsPhotoPushpinsVisible ? Visibility.Visible : Visibility.Collapsed;
            foreach (var photoPushpinViewModel in Pushpins)
            {
                photoPushpinViewModel.UpdateBackground();
                photoPushpinViewModel.Visibility = visibility;
            }
        }


        public void ChangePushpinCoordinate(TimeLineItemPushpinViewModel pushpin, GeoCoordinate coordinate)
        {
            if (pushpin != null)
            {
                var messages = pushpin.GetMessagesAndApplyCoordinate(coordinate);
                var photos = pushpin.GetPhotosAndApplyCoordinate(coordinate);

                photoService.UpdatePhotosLocations(photos, currentEvent, coordinate);
                messageService.UpdateMessages(messages, currentEvent);
            }
        }

        #region Event Handlers

        private void OnPlanItemSaved(PlanItemSavedMessage message)
        {
            planItemsPushpinsViewModel.SavePlanItem(message.OldPlanItem, message.PlanItem);
        }


        private void OnPlanItemDeleted(PlanItemDeletedMessage message)
        {
            PlanItemPushpinViewModel planItemPushpinToDelete =
               PlanItemPushpins.FirstOrDefault((p) => p.BasePlanItem.Equals(message.PlanItem));
            if (planItemPushpinToDelete != null)
            {
                planItemsPushpinsViewModel.DeletePushpin(planItemPushpinToDelete);
            }
        }


        private void OnLaunchCreatePlanItemFromRoutePushpinPage(LaunchCreatePlanItemFromRoutePushpinPageMessage message)
        {
            MessengerInstance.Send<CreatePlanItemFromRoutePushpinMessage>(new CreatePlanItemFromRoutePushpinMessage(message.RoutePushpin));
            navigationService.Navigate(EDIT_PLAN_ITEM_PATH);
        }


        private void OnShowPlanItems(ShowPlanItemPushpinsOnMapMessage message)
        {
            planItemsPushpinsViewModel.ShowPlanItems(message.PlanItems, IsPlanItemPushpinsVisible);
        }


        private void OnPhotoPushpinDeleted(PhotoPushpinDeletedMessage message)
        {
            TimeLineItemPushpinViewModel photoPushpinToDelete = null;
            foreach (var pushpin in Pushpins)
            {
                if (pushpin.Equals(message.Object))
                {
                    photoPushpinToDelete = pushpin;
                    break;
                }
            }

            if (photoPushpinToDelete != null)
            {
                var messages = photoPushpinToDelete.GetMessagesAndApplyCoordinate(null);
                var photos = photoPushpinToDelete.GetPhotosAndApplyCoordinate(null);

                photoService.UpdatePhotosLocations(photos, currentEvent, null);
                messageService.UpdateMessages(messages, currentEvent);
                photoPushpinToDelete.TimeLineItems = null;
                Pushpins.Remove(photoPushpinToDelete);
            }
        }


        private async void OnPhotoDeleted(DeletedEventPhotoMessage message)
        {
            if (await confirmationService.WaitConfirmation())
            {
                DeletePushpinItem(message.Photo);
            }
        }


        private async void OnMessageDeleted(MessageDeletedMessage message)
        {
            if (await confirmationService.WaitConfirmation())
            {
                DeletePushpinItem(message.Object);
            }
        }


        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            if (!message.Object.Equals(currentEvent))
            {
                currentEvent = message.Object;
            }
        }


        private void OnPhotosChanged(ShowPushpinItemsOnMapMessage message)
        {
            PushpinItems.Clear();
            var photoType = typeof(Photo);
            var messageType = typeof(Message);
            foreach (var timeLineItem in message.TimeLineItems)
            {
                if (timeLineItem.GetType() == photoType)
                {
                    PushpinItems.Add(new PhotoInMapViewModel((Photo)timeLineItem));
                }
                else if (timeLineItem.GetType() == messageType)
                {
                    PushpinItems.Add(new MessageInMapViewModel((Message)timeLineItem));
                }
            }
        }


        private void OnFindPushpinItemInPushpin(FindPushpinItemInPushpinMessage message)
        {
            var item = message.Object;

            foreach (var pushpin in Pushpins)
            {
                pushpin.SetSelected(false);
                foreach (var timeLineItem in pushpin.TimeLineItems)
                {
                    if (timeLineItem.Date == item.Date) // Dates are different across both photos and messages
                    {
                        pushpin.SetSelected(true);
                        MessengerInstance.Send<PushpinFoundMessage>(new PushpinFoundMessage(pushpin));
                    }
                }
            }
        }


        private void OnAddPhotosToPushpin(AddPhotosToPushpinMessage message)
        {
            selectedPushpin = message.Object;

            MessengerInstance.Send<ExcludePushpinPhotosChangedMessage>(new ExcludePushpinPhotosChangedMessage(GetExcludedTimeLineItemsFromPushpins(typeof(Photo))));
            navigationService.Navigate(PHOTO_LIST_CHOOSER, this.GetType().ToString());
        }


        private void OnAddMessagesToPushpin(AddMessagesToPushpinMessage message)
        {
            selectedPushpin = message.Object;

            MessengerInstance.Send<ExcludeMessagesFromListMessage>(new ExcludeMessagesFromListMessage(GetExcludedTimeLineItemsFromPushpins(typeof(Message))));
            navigationService.Navigate(MESSAGE_LIST_CHOOSER);
        }


        private void OnPhotosAddedToPushpin(EventPhotosChosenMessage message)
        {
            if (selectedPushpin != null)
            {
                photoService.UpdatePhotosLocations(message.Photos, currentEvent, selectedPushpin.Coordinate);
                foreach (var photo in message.Photos)
                {
                    photo.Coordinate = selectedPushpin.Coordinate;
                    FindAndAddPushpinItemViewModel(photo);
                }
            }
        }


        private void OnMessagesAddedToPushpin(MessagesChosenMessage msg)
        {
            if (selectedPushpin != null)
            {
                foreach (var message in msg.Objects)
                {
                    message.Coordinate = selectedPushpin.Coordinate;
                    messageService.UpdateMessage(message, currentEvent);
                    FindAndAddPushpinItemViewModel(message);
                }
            }
        }


        private void OnPhotoDeletedFromMap(PhotoDeletedFromMapMessage message)
        {
            var photo = message.Object;
            if (photo.Coordinate != null)
            {
                var photos = new List<Photo>();
                photos.Add(message.Object);
                photoService.UpdatePhotosLocations(photos, currentEvent, null);

                DeleteTimeLineItemFromPushpins(photo);
            }
        }


        private void OnMessageDeletedFromMap(DeleteMessageFromMapMessage msg)
        {
            var message = msg.Object;
            if (message.Coordinate != null)
            {
                message.Coordinate = null;
                messageService.UpdateMessage(message, currentEvent);

                DeleteTimeLineItemFromPushpins(message);
            }
        }


        private void OnViewPushpinTimeLineItems(ViewPushpinPhotosMessage message)
        {
            var timeLineItems = message.Object.TimeLineItems;
            if (timeLineItems.Count > 0)
            {
                MessengerInstance.Send<TimelineItemsViewerChangedMessage<IPushpinItem>>(new TimelineItemsViewerChangedMessage<IPushpinItem>(timeLineItems));
                navigationService.Navigate(PHOTO_VIEWER_PATH);
            }
            else
            {
                notificationService.Show("This pushpin does not have any items to view :(");
            }
        }


        private void OnRouteChosen(RouteChosenMessage message)
        {
            routePushpins.Clear();
            foreach (var route in message.Objects)
            {
                foreach (var pushpin in route.Pushpins)
                {
                    routePushpins.Add(new RoutePushpinViewModel(pushpin));
                }
            }
        }

        #endregion

        private void FindAndAddPushpinItemViewModel(IPushpinItem pushpinItem)
        {
            foreach (var pushpinItemViewModel in PushpinItems)
            {
                if (pushpinItemViewModel.DateItem.Id == pushpinItem.Id)
                {
                    selectedPushpin.AddTimeLineItem(pushpinItemViewModel);
                    break;
                }
            }
        }


        private ISet<long> GetExcludedTimeLineItemsFromPushpins(Type forType)
        {
            var excludedPhotosId = new HashSet<long>();
            foreach (var pushpinItemViewModel in PushpinItems)
            {
                if (pushpinItemViewModel.IsInPushpin && pushpinItemViewModel.DateItem.GetType().Equals(forType))
                {
                    excludedPhotosId.Add(pushpinItemViewModel.DateItem.Id);
                }
            }
            return excludedPhotosId;
        }


        private void DeletePushpinItem(IPushpinItem deletedPushpinItem)
        {
            BasePushpinItemInMapViewModel<IPushpinItem> viewModel = null;
            foreach (var pushpinItem in PushpinItems)
            {
                if (pushpinItem.DateItem.Equals(deletedPushpinItem))
                {
                    viewModel = pushpinItem;
                    break;
                }
            }

            if (viewModel != null)
            {
                PushpinItems.Remove(viewModel);
                if (viewModel.IsInPushpin)
                {
                    DeleteTimeLineItemFromPushpins(viewModel.DateItem);
                }
            }
        }


        private void DeleteTimeLineItemFromPushpins(IPushpinItem pushpinItem)
        {
            TimeLineItemPushpinViewModel foundPushpin = null;
            foreach (var pushpin in Pushpins)
            {
                if (pushpin.DeleteTimeLineItem(pushpinItem))
                {
                    foundPushpin = pushpin;
                    break;
                }
            }
            if (foundPushpin != null && foundPushpin.TimeLineItems.Count == 0)
            {
                pushpins.Remove(foundPushpin);
            }
        }


        private void UpdatePushpinsVisibility<T>(ICollection<T> pushpins, bool isVisible) where T : BasePushpinViewModel
        {
            foreach (var item in pushpins)
            {
                item.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        private void StopLocationWatcher()
        {
            if (currentLocationWatcher != null)
            {
                currentLocationWatcher.Stop();
                currentLocationWatcher.Dispose();
                currentLocationWatcher = null;
            }
        }
    }
}
