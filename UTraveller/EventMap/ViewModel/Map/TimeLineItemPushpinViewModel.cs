using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.EventMap.Messages;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel
{
    public class TimeLineItemPushpinViewModel : BasePushpinViewModel
    {

        public TimeLineItemPushpinViewModel(GeoCoordinate coordinate)
        {
            Coordinate = coordinate;
            DeletePushpinCommand = new ActionCommand(DeletePushpin);
            ViewTimeLineItemsCommand = new ActionCommand(ViewTimeLineItems);
            AddPhotosCommand = new ActionCommand(AddPhotos);
            AddMessagesCommand = new ActionCommand(AddMessages);
            
            SetSelected(false);
            TimeLineItems = new List<IPushpinItem>();
        }


        public ICommand ViewTimeLineItemsCommand
        {
            get;
            private set;
        }


        public ICommand AddPhotosCommand
        {
            get;
            private set;
        }


        public ICommand AddMessagesCommand
        {
            get;
            private set;
        } 


        public ICommand DeletePushpinCommand
        {
            get;
            private set;
        }


        public ICollection<IPushpinItem> TimeLineItems
        {
            get;
            set;
        }


        public Brush IconBackground
        {
            get;
            set;
        }


        public GeoCoordinate Coordinate
        {
            get;
            set;
        }


        public Brush Background
        {
            get
            {
                Brush brush = BackgroundColor;

                if (TimeLineItems.Count > 0)
                {
                    var firstItem = GetFirstPhotoOrDefault();
                    if (firstItem != null)
                    {
                        brush = new ImageBrush()
                        {
                            ImageSource = firstItem.Thumbnail
                        };
                    }
                }
                return brush;
            }
        }
        

        public BitmapImage Icon
        {
            get
            {
                if (TimeLineItems.Count > 0)
                {
                    var firstItem = GetFirstPhotoOrDefault();
                    if (firstItem == null)
                    {
                        return TimeLineItems.First().Thumbnail;
                    }
                }
                return null;
            }
        }


        public bool IsMessage
        {
            get
            {
                return TimeLineItems.Count > 0 && GetFirstPhotoOrDefault() == null;
            }
        }



        public bool IsPhoto
        {
            get
            {
                return TimeLineItems.Count > 0 && GetFirstPhotoOrDefault() != null;
            }
        }


        public string TimeLineItemsQuantity
        {
            get { return TimeLineItems.Count > 1 ? TimeLineItems.Count.ToString() : string.Empty; }
        }


        public List<Photo> GetPhotosAndApplyCoordinate(GeoCoordinate coordinate)
        {
            var photos = new List<Photo>();
            foreach (var timeLineItem in TimeLineItems)
            {
                if (timeLineItem is Photo)
                {
                    timeLineItem.Coordinate = coordinate;
                    photos.Add((Photo)timeLineItem);
                }
            }
            return photos;
        }


        public List<Message> GetMessagesAndApplyCoordinate(GeoCoordinate coordinate)
        {
            var messages = new List<Message>();
            foreach (var timeLineItem in TimeLineItems)
            {
                if (timeLineItem is Message)
                {
                    timeLineItem.Coordinate = coordinate;
                    messages.Add((Message)timeLineItem);
                }
            }
            return messages;
        }


        public void UpdateBackground()
        {
            RaisePropertyChanged("Background");
            RaisePropertyChanged("Icon");
        }


        public void SetSelected(bool isSelected)
        {
            if (isSelected)
            {
                IconBackground = MainColor;
            }
            else
            {
                IconBackground = BackgroundColor;
            }
            RaisePropertyChanged("IconBackground");
        }


        public void AddTimeLineItem<T>(BaseTimeLineItemViewModel<T> timeLineItem) where T : IPushpinItem
        {
            timeLineItem.DateItem.Coordinate = Coordinate;
            TimeLineItems.Add(timeLineItem.DateItem);
            UpdateBackground();
            UpdateQuantity();
            UpdateTimeLineDefinitionFlag();
        }


        public bool DeleteTimeLineItem(IPushpinItem timeLineItem)
        {
            bool isDeleted = TimeLineItems.Remove(timeLineItem);
            if (isDeleted)
            {
                timeLineItem.Coordinate = null;
                UpdateBackground();
                UpdateQuantity();
                UpdateTimeLineDefinitionFlag();
            }
            return isDeleted;
        }


        private void DeletePushpin()
        {
            MessengerInstance.Send<PhotoPushpinDeletedMessage>(new PhotoPushpinDeletedMessage(this));
        }


        private void ViewTimeLineItems()
        {
            MessengerInstance.Send<ViewPushpinPhotosMessage>(new ViewPushpinPhotosMessage(this));
        }


        private void AddPhotos()
        {
            MessengerInstance.Send<AddPhotosToPushpinMessage>(new AddPhotosToPushpinMessage(this));
        }


        private void AddMessages()
        {
            MessengerInstance.Send<AddMessagesToPushpinMessage>(new AddMessagesToPushpinMessage(this));
        }


        private IPushpinItem GetFirstPhotoOrDefault()
        {
            var firstItem = TimeLineItems.FirstOrDefault((item) => item as Photo != null);
            return firstItem;
        }


        private void UpdateQuantity()
        {
            RaisePropertyChanged("TimeLineItemsQuantity");
        }


        private void UpdateTimeLineDefinitionFlag()
        {
            RaisePropertyChanged("IsMessage");
            RaisePropertyChanged("IsPhoto");
        }
    }
}
