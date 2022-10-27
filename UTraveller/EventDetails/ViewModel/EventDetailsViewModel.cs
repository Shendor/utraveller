using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Live;
using Microsoft.Phone.Controls;
using Ninject;
using ServiceApi.UTraveller.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UTraveller.Common.Control;
using UTraveller.Common.Control.DateRangeEditor;
using UTraveller.Common.Control.Dialog;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.MessagePost.Messages;
using UTraveller.MoneySpendings.Message;
using UTraveller.MoneySpendings.ViewModel;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;
using Windows.System;

namespace UTraveller.EventDetails.ViewModel
{
    public class EventDetailsViewModel : BasePhotoListViewModel<ITimeLineItem<IDateItem>>
    {
        private static readonly string IMAGE_CHOOSER_PATH = "/ImageChooser/Control/ImageChooser.xaml";
        private static readonly string EVENT_MAP_PATH = "/EventMap/EventMapPage.xaml";
        private static readonly string PHOTO_VIEWER_PATH = "/PhotoViewer/PhotoViewerPage.xaml";
        private static readonly string MONEY_SPENDING_PATH = "/MoneySpending/MoneySpendingPage.xaml";
        private static readonly string MESSAGE_PATH = "/MessagePost/MessagePostPage.xaml";
        private static readonly string PHOTO_LIST_CHOOSER = "/ImageChooser/Control/PhotoListChooser.xaml";
        private static readonly string PLAN_ITEM_PATH = "/TripPlanEditor/EditPlanItemPage.xaml";
        private static readonly string DATE_RANGE_EDITOR_PATH = "/Common/Control/DateRangeEditor/DateRangeEditor.xaml";
        private static readonly string PHOTOS_QUANTITY_FORMAT = "{0} {1}";
        private static readonly string EDIT_DATE_RANGE_TOKEN = "EDIT_EVENT_DATE_RANGE_TOKEN";

        private ICommand showMapCommand;
        private Event currentEvent;
        private bool isEventChanged;
        private IImageLoaderService remotePhotoLoaderService;
        private IPhotoUploader photoUploader;
        private IMessageService messageService;
        private IImageService imageService;
        private IImageCropService imageCropService;
        private IEventService eventService;
        private IGeoCoordinateService geoLocationService;
        private IExpenseService moneySpendingService;
        private ICancelableTaskProgressService taskProgressService;
        private ITaskExecutionManager taskExecutionManager;
        private ITaskProgressService backgroundTaskProgressService;
        private IImageInitializer mediaLibraryPhotoTransferService;
        private IFacebookAuthService facebookAuthService;
        private NotificationService notificationService;
        private ConfirmationService confirmationService;
        private INetworkConnectionCheckService networkCheckService;
        private ITripBackupService backupService;
        private BitmapImage eventImage;
        private bool hasNoTimeLineItems;
        private bool hasNoMoneySpendings;

        public EventDetailsViewModel(INavigationService navigationService,
            IParameterContainer<string> parameterContainer, IPhotoService photoService,
             [Named("remotePhotoLoaderService")] IImageLoaderService remotePhotoLoaderService,
            IPhotoUploader photoUploader, IMessageService messageService, IImageService imageService,
            IEventService eventService, IImageCropService imageCropService, IGeoCoordinateService geoLocationService,
            IExpenseService moneySpendingService, NotificationService notificationService,
            ICancelableTaskProgressService dialogTaskProgressService,
            [Named("backgroundTaskProgressService")] ITaskProgressService backgroundTaskProgressService,
            [Named("mediaLibraryPhotoImageInitializer")] IImageInitializer mediaLibraryPhotoTransferService,
            ITaskExecutionManager taskExecutionManager, INetworkConnectionCheckService networkCheckService,
            ConfirmationService confirmationService, IFacebookAuthService facebookAuthService, TripPlanViewModel tripPlanViewModel,
            ITripBackupService backupService)
            : base(navigationService, parameterContainer, photoService)
        {
            this.remotePhotoLoaderService = remotePhotoLoaderService;
            this.photoUploader = photoUploader;
            this.messageService = messageService;
            this.imageService = imageService;
            this.eventService = eventService;
            this.imageCropService = imageCropService;
            this.geoLocationService = geoLocationService;
            this.moneySpendingService = moneySpendingService;
            this.notificationService = notificationService;
            this.taskProgressService = dialogTaskProgressService;
            this.taskExecutionManager = taskExecutionManager;
            this.backgroundTaskProgressService = backgroundTaskProgressService;
            this.mediaLibraryPhotoTransferService = mediaLibraryPhotoTransferService;
            this.networkCheckService = networkCheckService;
            this.confirmationService = confirmationService;
            this.facebookAuthService = facebookAuthService;
            this.TripPlanViewModel = tripPlanViewModel;
            this.backupService = backupService;

            TripPlanViewMode = TripPlanViewMode.Expense;
            eventImage = new BitmapImage();
            showMapCommand = new ActionCommand(ShowMap);
            AddTripPlanItemCommand = new ActionCommand(AddTripPlanItem);
            PostMessageCommand = new ActionCommand(PostMessage);
            ShowTimeLineItemsCommand = new ActionCommand(ShowTimeLineItems);
            BackupCommand = new ActionCommand(BackupTrip);

            MessengerInstance.Register<PhonePhotosChosenMessage>(this, OnPhotoAdded);
            MessengerInstance.Register<MessagePostedMessage>(this, GetType().ToString(), OnMessageAdded);
            MessengerInstance.Register<DeletedEventPhotoMessage>(this, OnPhotoDeleted);
            MessengerInstance.Register<MessageDeletedMessage>(this, OnMessageDeleted);
            MessengerInstance.Register<DeleteMoneySpendingMessage>(this, OnMoneySpendingDeleted);
            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);
            MessengerInstance.Register<EventPhotosChosenMessage>(this, GetType().ToString(), OnImageOfEventChanged);
            MessengerInstance.Register<MoneySpendingAddedMessage>(this, OnMoneySpendingAdded);
            MessengerInstance.Register<DateRangeChosenMessage>(this, EDIT_DATE_RANGE_TOKEN, OnEventDateRangeChanged);
        }

        public IList<GroupedEventPhotoViewModel> GroupedTimeLineItems
        {
            get;
            set;
        }


        public IEnumerable<MoneySpendingListViewModel> GroupedMoneySpendings
        {
            get;
            private set;
        }


        public TripPlanViewModel TripPlanViewModel
        {
            get;
            private set;
        }


        public TripPlanViewMode TripPlanViewMode
        {
            get;
            set;
        }


        public string Name
        {
            get { return (currentEvent != null) ? currentEvent.Name : null; }
            set
            {
                if (currentEvent != null)
                {
                    currentEvent.Name = value;
                    eventService.UpdateEvent(currentEvent);
                }
            }
        }


        public string PhotosQuantity
        {
            get
            {
                return currentEvent.PhotosQuantity < 0 ? "?" :
                    currentEvent.PhotosQuantity == 1 ?
                    string.Format(PHOTOS_QUANTITY_FORMAT, currentEvent.PhotosQuantity, AppResources.EventDetails_Photo) :
                    string.Format(PHOTOS_QUANTITY_FORMAT, currentEvent.PhotosQuantity, AppResources.EventDetails_Photos);
            }
        }


        public bool HasNoImage
        {
            get { return (currentEvent != null) ? currentEvent.Image == null : true; }
        }


        public string DateRange
        {
            get { return currentEvent != null ? currentEvent.DateRange : null; }
        }


        public bool HasNoTimeLineItems
        {
            get { return hasNoTimeLineItems; }
            set
            {
                hasNoTimeLineItems = value;
                RaisePropertyChanged("HasNoTimeLineItems");
            }
        }

        public bool HasNoMoneySpendings
        {
            get { return hasNoMoneySpendings; }
            set
            {
                hasNoMoneySpendings = value;
                RaisePropertyChanged("HasNoMoneySpendings");
            }
        }


        public BitmapImage EventImage
        {
            get
            {
                if (currentEvent != null && currentEvent.Image != null)
                {
                    imageService.WriteBytesToBitmapImage(currentEvent.Image, eventImage);
                }
                return eventImage;
            }
        }


        public void Initialize()
        {
            if (currentEvent != null && isEventChanged)
            {
                backgroundTaskProgressService.RunIndeterminateProgress();
                try
                {
                    var moneySpendings = moneySpendingService.GetExpenses(currentEvent);
                    InitializeTimeLineItems(currentEvent);
                    InitializeMoneySpendings(moneySpendings);
                    UpdateCurrentEvent();
                    TripPlanViewModel.Initialize(currentEvent);
                    isEventChanged = false;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error in loading timeline items: " + ex.Message);
                }
                finally
                {
                    backgroundTaskProgressService.FinishProgress();
                }
            }
            else
            {
                UpdateTimeLineItemsDescription();
            }
        }


        public override void Cleanup()
        {
            if (eventImage != null)
            {
                eventImage.UriSource = null;
            }
            HasNoTimeLineItems = false;
            HasNoMoneySpendings = false;
            if (TimeLineItems != null)
            {
                UpdateCurrentEvent();
            }

            GroupedTimeLineItems = null;
            TimeLineItems = null;
            GroupedMoneySpendings = null;
        }


        protected override void NavigateImageChooser()
        {
            var excludedPhotos = new List<Photo>();
            foreach (var photoThumbnail in TimeLineItems)
            {
                if (IsPhoto(photoThumbnail))
                {
                    excludedPhotos.Add(photoThumbnail.DateItem as Photo);
                }
            }

            MessengerInstance.Send<ExcludeEventPhotosChangedMessage>(new ExcludeEventPhotosChangedMessage(excludedPhotos));
            navigationService.Navigate(IMAGE_CHOOSER_PATH, this.GetType().ToString());
        }

        #region Commands

        public ICommand BackupCommand
        {
            get;
            private set;
        }


        public ICommand ShowMapCommand
        {
            get { return showMapCommand; }
        }


        public ICommand AddTripPlanItemCommand
        {
            get;
            private set;
        }


        public ICommand PostMessageCommand
        {
            get;
            private set;
        }


        public ICommand ShowTimeLineItemsCommand
        {
            get;
            private set;
        }


        private void ShowMap()
        {
            if (TimeLineItems != null)
            {
                var timeLineItems = new List<IDateItem>();
                foreach (var timeLineItem in TimeLineItems)
                {
                    timeLineItems.Add(timeLineItem.DateItem);
                }
                MessengerInstance.Send<ShowPushpinItemsOnMapMessage>(new ShowPushpinItemsOnMapMessage(timeLineItems));
                TripPlanViewModel.ShowPlanItemsOnMap();

                navigationService.Navigate(EVENT_MAP_PATH);
            }
        }


        public void ChangeEventImage()
        {
            navigationService.Navigate(PHOTO_LIST_CHOOSER, this.GetType().ToString());
        }


        public void ViewTimelineItems(IDateItem startFrom)
        {
            if (TimeLineItems != null)
            {
                // TODO try to optimize using SortedDictionary and get selected item by date
                var photos = new HashSet<IDateItem>();
                foreach (var photoThumbnail in TimeLineItems)
                {
                    photos.Add(photoThumbnail.DateItem);
                }
                MessengerInstance.Send<TimelineItemsViewerChangedMessage<IDateItem>>(new TimelineItemsViewerChangedMessage<IDateItem>(startFrom, photos));
                navigationService.Navigate(PHOTO_VIEWER_PATH);
            }
        }


        private void PostMessage()
        {
            navigationService.Navigate(MESSAGE_PATH, GetType().ToString());
        }


        private void AddTripPlanItem()
        {
            if (TripPlanViewMode == TripPlanViewMode.Expense)
            {
                navigationService.Navigate(MONEY_SPENDING_PATH);
            }
            else
            {
                navigationService.Navigate(PLAN_ITEM_PATH);
            }
        }


        private void ShowTimeLineItems()
        {
            ViewTimelineItems(null);
        }


        private async void BackupTrip()
        {
            if (await confirmationService.Show("Do you want to save this trip on your OneDrive?"))
            {
                try
                {
                    taskProgressService.RunIndeterminateProgress("Saving the trip...");
                    var isSuccess = await backupService.Backup(currentEvent);
                    if (isSuccess)
                    {
                        notificationService.Show("Your trip was successfully saved on your OneDrive :)");
                    }
                    else
                    {
                        notificationService.Show("Sorry, but this trip wasn't saved in your OneDrive :(");
                    }
                }
                catch (Exception ex)
                {
                    notificationService.Show("Sorry, we can't backup your trip :(");
                }
                finally
                {
                    taskProgressService.FinishProgress();
                }
            }
        }

        public void EditDateRange()
        {
            if (currentEvent != null)
            {
                MessengerInstance.Send<DateRangeToEditChangedMessage>(new DateRangeToEditChangedMessage(EDIT_DATE_RANGE_TOKEN, currentEvent.Date, currentEvent.EndDate));
                navigationService.Navigate(DATE_RANGE_EDITOR_PATH);
            }
        }
        #endregion

        #region Even handlers

        private async void OnImageOfEventChanged(EventPhotosChosenMessage message)
        {
            var photo = message.Photos.FirstOrDefault();
            if (currentEvent != null && photo != null)
            {
                taskProgressService.RunIndeterminateProgress("Setting image for the trip...");
                var stream = await remotePhotoLoaderService.Load(photo);
                if (taskProgressService.IsCanceled)
                {
                    return;
                }
                if (stream != null)
                {
                    try
                    {
                        currentEvent.Image = imageCropService.ChangeResolutionAndGetBytes(stream, 640, 360, 80);
                        eventService.UpdateEvent(currentEvent);
                        imageService.WriteBytesToBitmapImage(currentEvent.Image, eventImage);
                    }
                    catch (Exception)
                    {
                        notificationService.Show("Sorry, but we've got an error and can't set this image to the trip :(");
                    }
                    finally
                    {
                        taskProgressService.FinishProgress();
                    }
                }
                else
                {
                    taskProgressService.FinishProgress();
                    notificationService.Show("Image was not found on your device :(");
                }

            }
        }


        private async void OnPhotoAdded(PhonePhotosChosenMessage message)
        {
            var items = TimeLineItems;

            int progressStep = 1;
            foreach (var photo in message.Photos)
            {
                try
                {
                    taskProgressService.UpdateProgress(progressStep++, message.Photos.Count, "Adding photos to the trip...");
                    await photoService.AddPhoto(photo, currentEvent);
                    items.Add(new PhotoViewModel(photo));

                    if (taskProgressService.IsCanceled)
                    {
                        break;
                    }
                }
                catch (LimitExceedException ex)
                {
                    notificationService.Show(string.Format(AppResources.Limit_Exceeded, AppResources.Photo, ex.Limit));
                }
                catch (Exception ex)
                {
                    notificationService.Show(ex.Message);
                }
                finally
                {
                    mediaLibraryPhotoTransferService.DisposeImage(photo);
                }
            }
            CopyToTimeLineItemsCollection(items);
            //InitializeGroupedTimeLineItems();
            taskProgressService.FinishProgress();
        }


        public async void AddPhoto(string photoName)
        {
            taskProgressService.RunIndeterminateProgress("Adding photo to the trip...");
            try
            {
                var items = TimeLineItems;
                var photo = photoUploader.GetCameraPhotoByName(photoName);
                await photoService.AddPhoto(photo, currentEvent);
                items.Add(new PhotoViewModel(photo));
                CopyToTimeLineItemsCollection(items);
            }
            catch (LimitExceedException lex)
            {
                notificationService.Show(string.Format(AppResources.Limit_Exceeded, AppResources.Photo, lex.Limit));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot add photo: " + ex.Message);
            }
            finally
            {
                taskProgressService.FinishProgress();
            }
        }


        private async void OnMessageAdded(MessagePostedMessage msg)
        {
            if (currentEvent != null)
            {
                taskProgressService.RunIndeterminateProgress("Adding Message to trip...");
                try
                {
                    Message message = new Message();
                    message.Text = msg.Object;
                    await messageService.AddMessageToEvent(message, currentEvent);
                    var items = TimeLineItems;
                    items.Add(new MessageViewModel(message));
                    CopyToTimeLineItemsCollection(items);
                }
                catch (LimitExceedException lex)
                {
                    taskProgressService.FinishProgress();
                    notificationService.Show(string.Format(AppResources.Limit_Exceeded, AppResources.Message, lex.Limit));
                }
                finally
                {
                    taskProgressService.FinishProgress();
                }
            }
        }


        private void OnMoneySpendingAdded(MoneySpendingAddedMessage message)
        {
            InitializeMoneySpendings(moneySpendingService.GetExpenses(currentEvent));
        }


        private async void OnPhotoDeleted(DeletedEventPhotoMessage message)
        {
            if (await confirmationService.Show("Do you want to delete this photo?"))
            {
                ITimeLineItem<IDateItem> deletedPhotoItem = null;

                foreach (var photoThumbnail in TimeLineItems)
                {
                    if (photoThumbnail.DateItem.Equals(message.Photo))
                    {
                        bool confirmDelete = true;
                        if (networkCheckService.HasConnection &&
                           message.Photo.FacebookPhotoId != null &&
                           !facebookAuthService.IsSignedIn())
                        {
                            confirmDelete =
                                await confirmationService.Show("This photo will not be deleted from uTraveler facebook album " +
                                "because you are not signed in to Facebook. \nWould you like to continue?");
                        }
                        if (confirmDelete)
                        {
                            deletedPhotoItem = photoThumbnail;
                            photoService.DeletePhoto(message.Photo, currentEvent);
                        }
                        break;
                    }
                }
                var items = TimeLineItems;
                items.Remove(deletedPhotoItem);
                CopyToTimeLineItemsCollection(items);
            }
        }


        private async void OnMessageDeleted(MessageDeletedMessage message)
        {
            if (await confirmationService.Show("Do you want to delete this message?"))
            {
                ITimeLineItem<IDateItem> deletedTimeLineItem = null;
                foreach (var photoThumbnail in TimeLineItems)
                {
                    if (photoThumbnail.DateItem.Equals(message.Object))
                    {
                        deletedTimeLineItem = photoThumbnail;
                        messageService.DeleteMessage(message.Object, currentEvent);
                        break;
                    }
                }
                var items = TimeLineItems;
                items.Remove(deletedTimeLineItem);
                CopyToTimeLineItemsCollection(items);
            }
        }


        private async void OnMoneySpendingDeleted(DeleteMoneySpendingMessage message)
        {
            if (await confirmationService.Show("Do you want to delete this item?"))
            {
                moneySpendingService.DeleteExpense(message.Object, currentEvent);
                var moneySpendings = moneySpendingService.GetExpenses(currentEvent);
                InitializeMoneySpendings(moneySpendings);
            }
        }


        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            if (!message.Object.Equals(currentEvent))
            {
                currentEvent = message.Object;
                isEventChanged = true;
            }
        }

        private void OnEventDateRangeChanged(DateRangeChosenMessage message)
        {
            if (currentEvent != null)
            {
                currentEvent.Date = message.StartDate;
                currentEvent.EndDate = message.EndDate;
                UpdateCurrentEvent();
            }
        }


        #endregion


        private void UpdateTimeLineItemsDescription()
        {
            if (TimeLineItems != null)
            {
                foreach (var photoThumbnail in TimeLineItems)
                {
                    if (photoThumbnail is IDescriptionedTimeLineItem)
                    {
                        ((IDescriptionedTimeLineItem)photoThumbnail).UpdateDescription();
                    }
                }
            }
        }


        private void UpdateCurrentEvent()
        {
            if (TimeLineItems != null)
            {
                currentEvent.PhotosQuantity = 0;
                foreach (var item in TimeLineItems)
                {
                    currentEvent.PhotosQuantity += (item.DateItem as Photo == null) ? 0 : 1;
                }

                eventService.UpdateEvent(currentEvent);
                RaisePropertyChanged("PhotosQuantity");
                RaisePropertyChanged("DateRange");
            }
        }


        private void InitializeTimeLineItems(Event e)
        {
            var photos = photoService.GetPhotos(e);
            var messages = messageService.GetMessagesOfEvent(e);

            TimeLineItems = new SortedSet<ITimeLineItem<IDateItem>>();
            foreach (var photo in photos)
            {
                TimeLineItems.Add(new PhotoViewModel(photo));
            }

            foreach (var message in messages)
            {
                TimeLineItems.Add(new MessageViewModel(message));
            }
            InitializeGroupedTimeLineItems();

            RaisePropertyChanged("TimeLineItems");
            RaisePropertyChanged("PhotosQuantity");
        }

        private void InitializeGroupedTimeLineItems()
        {
            //TODO: load this collection on demand, when user changes view
            GroupedTimeLineItems = new List<GroupedEventPhotoViewModel>(GroupPictures(TimeLineItems));
            RaisePropertyChanged("GroupedTimeLineItems");
        }


        private IEnumerable<GroupedEventPhotoViewModel> GroupPictures(ICollection<ITimeLineItem<IDateItem>> photos)
        {
            var groupedPhotos =
                from photo in photos
                orderby photo.Date descending
                group photo by photo.Date.ToString(AppResources.Short_Dayly_Date_Format) into photosByDay
                select new GroupedEventPhotoViewModel(photosByDay);
            return groupedPhotos;
        }


        private void InitializeMoneySpendings(IEnumerable<MoneySpending> moneySpendings)
        {
            GroupedMoneySpendings = GroupMoneySpendings(moneySpendings);
            RaisePropertyChanged("GroupedMoneySpendings");
            UpdateEventItemsFlags();
        }


        private IEnumerable<MoneySpendingListViewModel> GroupMoneySpendings(IEnumerable<MoneySpending> moneySpendings)
        {
            return from moneySpending in moneySpendings
                   orderby moneySpending.Date descending
                   group new MoneySpendingItemViewModel(moneySpending) by moneySpending.Date.ToString(AppResources.Short_Dayly_Date_Format) into moneySpendingsByDay
                   select new MoneySpendingListViewModel(moneySpendingsByDay);
        }


        private void CopyToTimeLineItemsCollection(ICollection<ITimeLineItem<IDateItem>> items)
        {
            TimeLineItems = new SortedSet<ITimeLineItem<IDateItem>>();
            foreach (var item in items)
            {
                TimeLineItems.Add(item);
            }

            RaisePropertyChanged("TimeLineItems");
            InitializeGroupedTimeLineItems();
            UpdateEventItemsFlags();
            UpdateCurrentEvent();
        }

        private void UpdateEventItemsFlags()
        {
            HasNoTimeLineItems = TimeLineItems == null || TimeLineItems.Count == 0;
            HasNoMoneySpendings = GroupedMoneySpendings == null || GroupedMoneySpendings.Count() == 0;
        }


        private bool IsPhoto(ITimeLineItem<IDateItem> timeLineItem)
        {
            return timeLineItem.DateItem.GetType() == typeof(Photo);
        }
    }
}
