using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventList.ViewModel
{
    public class EventTileViewModel : BaseViewModel, IComparable<EventTileViewModel>, IDisposable
    {
        private Event e;
        private BitmapImage thumbnail;
        private int width;
        private int height;
        private Brush color;

        public EventTileViewModel(Event e)
        {
            this.e = e;
            thumbnail = new BitmapImage();
            Color currentAccentColor = (Color)App.Current.Resources["PhoneAccentColor"];
            color = new SolidColorBrush(currentAccentColor);
            width = 150;
            height = 150;
            DeleteEventCommand = new ActionCommand(DeleteEvent);
            ShowEventDetailsCommand = new ActionCommand(ShowEventDetails);
            CurrentEventCommand = new ActionCommand(SetCurrentEvent);
        }


        public ICommand DeleteEventCommand
        {
            get;
            private set;
        }


        public ICommand ShowEventDetailsCommand
        {
            get;
            private set;
        }


        public ICommand CurrentEventCommand
        {
            get;
            private set;
        }


        public string CurrentItemLabel
        {
            get { return Event.IsCurrent ? AppResources.EventItem_MenuItem_UnsetCurrent : AppResources.EventItem_MenuItem_SetCurrent; }
        }


        public string Name
        {
            get { return Event.Name; }
        }


        public string DateRange
        {
            get { return Event.DateRange; }
        }


        public BitmapImage Thumbnail
        {
            get { return thumbnail; }
            set { thumbnail = value; }
        }


        public bool HasNoImage
        {
            get { return e.Image == null; }
        }


        public string PhotosQuantity
        {
            get { return e.PhotosQuantity >= 0 ? e.PhotosQuantity.ToString() : "?"; }
        }


        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        public int Height
        {
            get { return height; }
            set { height = value; }
        }


        public Brush Color
        {
            get { return color; }
            set { color = value; }
        }


        public Event Event
        {
            get { return e; }
        }


        private void DeleteEvent()
        {
            MessengerInstance.Send<DeletedEventMessage>(new DeletedEventMessage(Event));
        }


        private void ShowEventDetails()
        {
            MessengerInstance.Send<EventSelectionChangedMessage>(new EventSelectionChangedMessage(Event));
        }


        private void SetCurrentEvent()
        {
            Event.IsCurrent = !Event.IsCurrent;
            MessengerInstance.Send<EventCurrentChangedMessage>(new EventCurrentChangedMessage(Event));
            RaisePropertyChanged("CurrentItemLabel");
        }


        public void Dispose()
        {
            thumbnail.UriSource = null;
        }


        internal void UnsetCurrentStatus()
        {
            Event.IsCurrent = false;
            RaisePropertyChanged("CurrentItemLabel");
        }


        public int CompareTo(EventTileViewModel other)
        {
            return -Event.Date.CompareTo(other.Event.Date);
        }
    }
}
