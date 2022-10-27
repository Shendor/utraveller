using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Phone.Net.NetworkInformation;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Control;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.MessagePost.Messages;
using UTraveller.PhotoViewer.Messages;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTraveller.Service.Model;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.PhotoViewer.ViewModel
{
    public class PhotoViewerViewModel : BaseViewModel
    {
        private static readonly string MESSAGE_PATH = "/MessagePost/MessagePostPage.xaml";

        private ICollection<DetailedTimelineItemViewModel> timeLineItems;
        private IFacebookClientService facebookClientService;
        private IFacebookAuthService facebookAuthService;
        private IImageService imageService;
        private INavigationService navigationService;
        private IPhotoService photoService;
        private IMessageService messageService;
        private INetworkConnectionCheckService networkConnectionCheckService;
        private NotificationService notificationService;
        private ITaskProgressService dialogTaskProgressService;
        private IImageInitializer imageInitializer;
        private FacebookPaging commentPaging;
        private Event currentEvent;

        public PhotoViewerViewModel(IFacebookClientService facebookClientService,
            IImageService imageService, INavigationService navigationService, IPhotoService photoService,
            IMessageService messageService, INetworkConnectionCheckService networkConnectionCheckService,
            NotificationService notificationService, [Named("mediaLibraryPhotoImageInitializer")] IImageInitializer imageInitializer,
            ICancelableTaskProgressService dialogTaskProgressService, IFacebookAuthService facebookAuthService)
        {
            this.facebookClientService = facebookClientService;
            this.imageService = imageService;
            this.navigationService = navigationService;
            this.photoService = photoService;
            this.messageService = messageService;
            this.networkConnectionCheckService = networkConnectionCheckService;
            this.notificationService = notificationService;
            this.dialogTaskProgressService = dialogTaskProgressService;
            this.imageInitializer = imageInitializer;
            this.facebookAuthService = facebookAuthService;
            this.timeLineItems = new List<DetailedTimelineItemViewModel>();

            commentPaging = new FacebookPaging();
            FacebookComments = new ObservableCollection<FacebookCommentViewModel>();
            FacebookLikes = new ObservableCollection<FacebookLikeViewModel>();
            Albums = new ObservableCollection<FacebookAlbum>();
            PostPhotoCommand = new ActionCommand(PostPhoto);
            PostMessageCommand = new ActionCommand(PostMessage);
            CommentPostCommand = new ActionCommand(CommentPost);
            NextCommentsCommand = new ActionCommand(NextComments);
            PreviousCommentsCommand = new ActionCommand(PreviousComments);

            InitializePrivacyTypes();

            MessengerInstance.Register<TimelineItemsViewerChangedMessage<IDateItem>>(this, OnTimeLineItemsForViewerChanged);
            MessengerInstance.Register<TimelineItemsViewerChangedMessage<IPushpinItem>>(this, OnTimeLineItemsForViewerChanged);
            MessengerInstance.Register<MessagePostedMessage>(this, GetType().ToString(), OnMessageAdded);
            MessengerInstance.Register<NetworkStatusChangedMessage>(this, OnNetworkStatusChanged);
            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);
        }


        public override void Cleanup()
        {
            foreach (var timeLineItem in TimeLineItems)
            {
                var photo = timeLineItem.DateItem as Photo;
                if (photo != null)
                {
                    imageInitializer.DisposeImage(photo);
                }
            }

            FacebookComments.Clear();
            FacebookLikes.Clear();
            Albums.Clear();
            TimeLineItems.Clear();
            timeLineItems = null;
            SelectedTimelineItem = null;
            SelectedAlbum = null;
            SelectedPrivacyType = null;
        }


        public async void Initialize()
        {
            if (DeviceNetworkInformation.IsNetworkAvailable && facebookAuthService.IsSignedIn())
            {
                await InitializeAlbums();
            }
        }


        public ICollection<DetailedTimelineItemViewModel> TimeLineItems
        {
            get { return timeLineItems; }
        }


        public ICollection<FacebookAlbum> Albums
        {
            get;
            private set;
        }


        public FacebookAlbum SelectedAlbum
        {
            get;
            set;
        }


        public ICollection<FacebookPrivacyModel> FacebookPrivacyTypes
        {
            get;
            private set;
        }


        public FacebookPrivacyModel SelectedPrivacyType
        {
            get;
            set;
        }


        public FacebookPrivacyModel SelectedMessagePrivacyType
        {
            get;
            set;
        }


        public string Date
        {
            get
            {
                return (SelectedTimelineItem != null) ? SelectedTimelineItem.FullFormattedDate : null;
            }
        }


        public string Comment
        {
            get;
            set;
        }


        public string CommentText
        {
            get;
            set;
        }


        public long TotalComments
        {
            get;
            set;
        }


        public long TotalLikes
        {
            get;
            set;
        }


        public bool HasNoLikes
        {
            get { return TotalLikes <= 0; }
        }


        public bool HasLikes
        {
            get { return !HasNoLikes; }
        }


        public bool HasManyComments
        {
            get;
            private set;
        }


        public bool IsPosted
        {
            get;
            private set;
        }


        public bool IsNotPosted
        {
            get { return !IsPosted; }
        }


        public bool IsConnectedToFacebook
        {
            get { return facebookAuthService.IsSignedIn(); }
        }


        public ICollection<FacebookCommentViewModel> FacebookComments
        {
            get;
            private set;
        }


        public ICollection<FacebookLikeViewModel> FacebookLikes
        {
            get;
            private set;
        }


        public ICommand PostPhotoCommand
        {
            get;
            private set;
        }


        public ICommand PostMessageCommand
        {
            get;
            private set;
        }


        public ICommand CommentPostCommand
        {
            get;
            private set;
        }


        public ICommand NextCommentsCommand
        {
            get;
            private set;
        }


        public ICommand PreviousCommentsCommand
        {
            get;
            private set;
        }


        public DetailedTimelineItemViewModel SelectedTimelineItem
        {
            get;
            set;
        }


        public void ShowNoContentDialog()
        {
            notificationService.Show("You don't have here anything to view :(");
        }


        public bool IsPhotoSelected()
        {
            return SelectedTimelineItem != null && SelectedTimelineItem.DateItem is Photo;
        }


        public bool IsMessageSelected()
        {
            return SelectedTimelineItem != null && SelectedTimelineItem.DateItem is Message;
        }


        public void WriteDescription()
        {
            string text = null;
            if (IsPhotoSelected())
            {
                text = ((Photo)SelectedTimelineItem.DateItem).Description;
            }
            else if (IsMessageSelected())
            {
                text = ((Message)SelectedTimelineItem.DateItem).Text;
            }
            MessengerInstance.Send<MessageChangedMessage>(new MessageChangedMessage(text));
            navigationService.Navigate(MESSAGE_PATH, GetType().ToString());
        }


        public async void SelectTimelineItem(DetailedTimelineItemViewModel timeLineItem)
        {
            SelectedTimelineItem = timeLineItem;
            if (networkConnectionCheckService.HasConnection)
            {
                if (IsPhotoSelected())
                {
                    IsPosted = facebookClientService.IsPhotoPosted(timeLineItem.Id);
                    Comment = timeLineItem.Text;
                }
                else if (IsMessageSelected())
                {
                    IsPosted = facebookClientService.IsMessagePosted(timeLineItem.Id);
                }
                if (IsPosted)
                {
                    await InitializeComments();
                    await InitializeLikes();
                }
            }
            else
            {
                IsPosted = false;
            }

            RaiseSocialPropertiesChanged();
        }


        public async Task InitializeComments()
        {
            if (IsPhotoSelected())
            {
                InitializeComments(await facebookClientService.GetPhotoComments(SelectedTimelineItem.Id));
            }
            else if (IsMessageSelected())
            {
                InitializeComments(await facebookClientService.GetMessageComments(SelectedTimelineItem.Id));
            }
        }


        private async Task InitializeLikes()
        {
            if (IsPhotoSelected())
            {
                InitializeLikes(await facebookClientService.GetPhotoLikes(SelectedTimelineItem.Id));
            }
            else if (IsMessageSelected())
            {
                InitializeLikes(await facebookClientService.GetMessageLikes(SelectedTimelineItem.Id));
            }
        }


        private void InitializeLikes(FacebookLikes likes)
        {
            TotalLikes = 0;
            FacebookLikes.Clear();
            if (likes != null)
            {
                if (likes.Summary != null)
                {
                    TotalLikes = likes.Summary.Total_count;
                    RaisePropertyChanged("TotalLikes");
                    RaisePropertyChanged("HasNoLikes");
                }
                foreach (var like in likes.Data)
                {
                    FacebookLikes.Add(new FacebookLikeViewModel(like));
                }
            }
        }


        private async void NextComments()
        {
            if (commentPaging.Next != null)
            {
                InitializeComments(await facebookClientService.GetComments(commentPaging.Next));
            }
        }


        private async void PreviousComments()
        {
            if (commentPaging.Previous != null)
            {
                InitializeComments(await facebookClientService.GetComments(commentPaging.Previous));
            }
        }


        private void InitializeComments(FacebookComments facebookComments)
        {
            TotalComments = 0;
            HasManyComments = false;
            FacebookComments.Clear();
            if (facebookComments != null)
            {
                if (facebookComments.Paging != null)
                {
                    commentPaging.Next = facebookComments.Paging.Next;
                    commentPaging.Previous = facebookComments.Paging.Previous;
                    HasManyComments = commentPaging.Next != null || commentPaging.Previous != null;
                }
                if (facebookComments.Summary != null)
                {
                    TotalComments = facebookComments.Summary.Total_count;
                    RaisePropertyChanged("TotalComments");
                    RaisePropertyChanged("HasManyComments");
                }
                foreach (var facebookComment in facebookComments.Data)
                {
                    FacebookComments.Add(new FacebookCommentViewModel(facebookComment));
                }
            }
        }


        private async void PostPhoto()
        {
            if (SelectedAlbum != null && SelectedPrivacyType != null && IsPhotoSelected())
            {
                var photo = (Photo)SelectedTimelineItem.DateItem;
                if (photo.ImageStream == null)
                {
                    notificationService.Show("Sorry, but you need to have this photo on your device in order to post in Facebook :(");
                }
                else
                {
                    var data = new SocialPostImageData();
                    data.DataId = SelectedTimelineItem.Id;
                    data.ImageContent = imageService.ToBytes(photo.ImageStream);
                    data.FileName = photo.Name;
                    data.Name = Comment != null ? Comment.Replace('\r', '\n') : "";
                    data.PrivacyType = SelectedPrivacyType.PrivacyType;

                    try
                    {
                        dialogTaskProgressService.RunIndeterminateProgress("Posting to Facebook...");

                        var result = await facebookClientService.PostPhotoToAlbum(SelectedAlbum, data);
                        dialogTaskProgressService.FinishProgress();
                        if (result != null)
                        {
                            IsPosted = true;
                            photo.FacebookPostId = result;
                            photoService.UpdatePhoto(photo, currentEvent);
                            RaiseSocialPropertiesChanged();
                            MessengerInstance.Send<TimeLineItemPostedMessage>(new TimeLineItemPostedMessage(true));
                            notificationService.Show("Photo has been posted successfully :)");
                            ResetFacebookInfoForCurrentPost();
                        }
                    }
                    catch (Exception ex)
                    {
                        dialogTaskProgressService.FinishProgress();
                    }
                    finally
                    {
                        if (string.IsNullOrEmpty(photo.FacebookPostId))
                        {
                            notificationService.Show(string.Format("Sorry, but you cannot post this Photo on album '{0}' because Facebook returned an error :(", SelectedAlbum.Name));
                        }
                    }
                }
            }
        }


        private async void PostMessage()
        {
            if (SelectedMessagePrivacyType != null && IsMessageSelected())
            {
                var message = (Message)SelectedTimelineItem.DateItem;
                var data = new SocialPostData();
                data.DataId = SelectedTimelineItem.Id;
                data.Name = data.Message = message.Text != null ? message.Text.Replace('\r', '\n') : "";
                data.PrivacyType = SelectedMessagePrivacyType.PrivacyType;

                try
                {
                    dialogTaskProgressService.RunIndeterminateProgress("Posting to Facebook...");
                    var result = await facebookClientService.PostStatus(data);
                    dialogTaskProgressService.FinishProgress();
                    if (result != null)
                    {
                        IsPosted = true;
                        message.FacebookPostId = result;
                        messageService.UpdateMessage(message, currentEvent);
                        RaiseSocialPropertiesChanged();
                        MessengerInstance.Send<TimeLineItemPostedMessage>(new TimeLineItemPostedMessage(true));
                        notificationService.Show("Message has been posted successfully :)");
                        ResetFacebookInfoForCurrentPost();
                    }
                }
                catch (Exception ex)
                {
                    dialogTaskProgressService.FinishProgress();
                }
                finally
                {
                    if (string.IsNullOrEmpty(message.FacebookPostId))
                    {
                        notificationService.Show("Sorry, but you cannot post this Message because Facebook returned an error :(");
                    }
                }
            }
        }


        private async void CommentPost()
        {
            if (SelectedTimelineItem != null)
            {

                dialogTaskProgressService.RunIndeterminateProgress("Commenting post...");
                try
                {
                    if (IsPhotoSelected())
                    {
                        await facebookClientService.CommentPhotoPost(SelectedTimelineItem.Id, CommentText);
                    }
                    else if (IsMessageSelected())
                    {
                        await facebookClientService.CommentMessagePost(SelectedTimelineItem.Id, CommentText);
                    }
                    await InitializeComments();
                    dialogTaskProgressService.FinishProgress();
                }
                catch (Exception)
                {
                    dialogTaskProgressService.FinishProgress();
                    notificationService.Show("Sorry, but you cannot comment this Post because Facebook returned an error :(");
                }
                CommentText = null;
                RaisePropertyChanged("CommentText");
            }
        }


        private void OnTimeLineItemsForViewerChanged<T>(TimelineItemsViewerChangedMessage<T> message) where T : IDateItem
        {
            timeLineItems = new List<DetailedTimelineItemViewModel>();
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            int positionNumber = 0;
            foreach (var photo in message.Objects)
            {
                var timeLineItem = new DetailedTimelineItemViewModel(positionNumber++, photo);
                if (message.StartFrom != null)
                {
                    timeLineItem.IsSelected = message.StartFrom.Equals(photo);
                }
                TimeLineItems.Add(timeLineItem);
            }
            if (message.StartFrom == null && TimeLineItems.Count > 0)
            {
                TimeLineItems.First().IsSelected = true;
            }
            watch.Stop();
            System.Diagnostics.Debug.WriteLine(watch.ElapsedTicks);
        }


        private void OnMessageAdded(MessagePostedMessage message)
        {
            if (IsPhotoSelected())
            {
                var photo = (Photo)SelectedTimelineItem.DateItem;
                photo.Description = message.Object;
                photoService.UpdatePhoto(photo, currentEvent);
            }
            else if (IsMessageSelected())
            {
                var msg = (Message)SelectedTimelineItem.DateItem;
                msg.Text = message.Object;
                messageService.UpdateMessage(msg, currentEvent);
            }
            SelectedTimelineItem.UpdateDescription();
        }


        private void OnNetworkStatusChanged(NetworkStatusChangedMessage message)
        {
            SelectTimelineItem(SelectedTimelineItem);
        }


        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            currentEvent = message.Object;
        }


        private async Task InitializeAlbums()
        {
            try
            {
                var albums = await facebookClientService.GetAlbums();
                if (albums != null)
                {
                    if (albums.FirstOrDefault((a) => a.Name.Equals(currentEvent.Name)) == null)
                    {
                        var eventAlbum = new FacebookAlbum();
                        eventAlbum.Name = currentEvent.Name;
                        Albums.Add(eventAlbum);
                    }
                    foreach (var album in albums)
                    {
                        Albums.Add(album);
                    }
                    if (Albums.Count > 0)
                    {
                        SelectedAlbum = Albums.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }


        private void InitializePrivacyTypes()
        {
            FacebookPrivacyTypes = new List<FacebookPrivacyModel>();
            foreach (var enumValue in Enum.GetValues(typeof(FacebookPrivacyType)))
            {
                var privacyType = (FacebookPrivacyType)enumValue;
                var name = AppResources.ResourceManager.GetString("Facebook_Privacy_" + privacyType);
                FacebookPrivacyTypes.Add(new FacebookPrivacyModel(name, privacyType));
            }
        }


        private void ResetFacebookInfoForCurrentPost()
        {
            TotalComments = 0;
            TotalLikes = 0;
            RaiseSocialPropertiesChanged();
        }


        private void RaiseSocialPropertiesChanged()
        {
            RaisePropertyChanged("IsPosted");
            RaisePropertyChanged("IsNotPosted");
            RaisePropertyChanged("Date");
            RaisePropertyChanged("Comment");
            RaisePropertyChanged("TotalLikes");
            RaisePropertyChanged("TotalComments");
            RaisePropertyChanged("HasManyComments");
            RaisePropertyChanged("HasLikes");
            RaisePropertyChanged("HasNoLikes");
        }
    }
}