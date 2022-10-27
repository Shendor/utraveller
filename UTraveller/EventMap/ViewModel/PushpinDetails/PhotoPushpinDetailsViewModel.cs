using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel
{
    public class PhotoPushpinDetailsViewModel : BasePhotoListViewModel<PushpinPhotoViewModel>
    {
        private static readonly string PHOTO_LIST_CHOOSER = "/ImageChooser/Control/PhotoListChooser.xaml";

        private ICommand deletePushpinCommand;
        private PhotoPushpin photoPushpin;
        private bool isPushpinChanged;
        private Event currentEvent;
        private ICollection<long> excludedPhotosId;

        public PhotoPushpinDetailsViewModel(INavigationService navigationService,
            IParameterContainer<string> parameterContainer,
            IPhotoService photoService)
            : base(navigationService, parameterContainer, photoService)
        {
            deletePushpinCommand = new ActionCommand(DeletePushpin);
            excludedPhotosId = new HashSet<long>();

            MessengerInstance.Register<DeletedPushpinPhotoMessage>(this, OnPhotoDeleted);
            MessengerInstance.Register<EventPhotosChosenMessage>(this, OnPhotoAdded);
            MessengerInstance.Register<ObjectMessage<PhotoPushpin>>(this, OnPhotoPushpinChanged);
            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);
        }

        public override void Cleanup()
        {
            excludedPhotosId.Clear();
            TimeLineItems.Clear();
        }

        public void Initialize()
        {
            if (photoPushpin != null && isPushpinChanged)
            {
                InitializePhotos(photoPushpin);
                isPushpinChanged = false;
            }
        }

        private void InitializePhotos(PhotoPushpin photoPushpin)
        {
          
        }

        protected override void NavigateImageChooser()
        {
           
        }

        #region Commands

        public ICommand DeletePushpinCommand
        {
            get { return deletePushpinCommand; }
        }

        private void DeletePushpin()
        {
            //MessengerInstance.Send<PhotoPushpinDeletedMessage>(new PhotoPushpinDeletedMessage(photoPushpin));
            photoPushpin = null;
        }

        #endregion

        #region Event Handlers

        private void OnPhotoAdded(EventPhotosChosenMessage message)
        {
            foreach (var photo in message.Photos)
            {
                TimeLineItems.Add(new PushpinPhotoViewModel(photo));
                excludedPhotosId.Add(photo.Id);
            }
        }

        private void OnPhotoDeleted(DeletedPushpinPhotoMessage message)
        {
            PushpinPhotoViewModel deletedPhotoItem = null;

            foreach (var photoThumbnail in TimeLineItems)
            {
                if (photoThumbnail.DateItem.Equals(message.Photo))
                {
                    deletedPhotoItem = photoThumbnail;
                    break;
                }
            }
            TimeLineItems.Remove(deletedPhotoItem);
        }

        private void OnPhotoPushpinChanged(ObjectMessage<PhotoPushpin> message)
        {
            if (!message.Object.Equals(photoPushpin))
            {
                photoPushpin = message.Object;
                isPushpinChanged = true;
            }
        }

        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            currentEvent = message.Object;
        }

        #endregion
    }
}
