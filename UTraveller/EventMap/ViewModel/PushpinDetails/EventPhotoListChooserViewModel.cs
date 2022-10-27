using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.ImageChooser.Model;
using UTraveller.ImageChooser.ViewModel;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel
{
    public class EventPhotoListChooserViewModel : PhotoListChooserViewModel
    {
        private Event currentEvent;
        private IPhotoService photoService;
        private ICollection<long> excludedPhotosId;

        public EventPhotoListChooserViewModel(INavigationService navigationService,
            IParameterContainer<string> parameterContainer, IPhotoService photoService)
            : base(navigationService)
        {
            this.photoService = photoService;
            photoListViewModel = new PhotoListViewModel(navigationService);

            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);
            MessengerInstance.Register<ExcludePushpinPhotosChangedMessage>(this, OnExcludePushpinPhotosChanged);
        }


        public override void Initialize()
        {
            if (currentEvent != null)
            {
                InitializePhotos(currentEvent);
            }
        }


        public override void Cleanup()
        {
            photoListViewModel = null;
            excludedPhotosId = null;
        }


        private void InitializePhotos(Event e)
        {
            var photos = photoService.GetPhotos(e);
            PhotoList = new PhotoListViewModel(navigationService);
            foreach (var photo in photos)
            {
                if (excludedPhotosId == null || excludedPhotosId.Where(id => id.Equals(photo.Id)).Count() == 0)
                {
                    PhotoList.Add(new CheckedImageModel(photo));
                }
            }

            RaisePropertyChanged("PhotoList");
        }


        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            if (!message.Object.Equals(currentEvent))
            {
                currentEvent = message.Object;
            }
        }


        private void OnExcludePushpinPhotosChanged(ExcludePushpinPhotosChangedMessage message)
        {
            excludedPhotosId = message.Objects;
        }


        protected override void SendPhotoChosenMessage(ICollection<Photo> choosedPhotos, object token)
        {
            MessengerInstance.Send<EventPhotosChosenMessage>(new EventPhotosChosenMessage(choosedPhotos), token);
        }
    }
}
