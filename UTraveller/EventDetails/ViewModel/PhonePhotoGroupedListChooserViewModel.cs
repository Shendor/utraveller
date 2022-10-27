using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTraveller.ImageChooser.ViewModel;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class PhonePhotoGroupedListChooserViewModel : ImageChooserViewModel
    {
        private ICollection<Photo> excludedPhotos;

        public PhonePhotoGroupedListChooserViewModel(IPhotoUploader photoUploader,
            INavigationService navigationService,
             [Named("backgroundTaskProgressService")] ITaskProgressService backgroundTaskProgressService)
            : base(photoUploader, navigationService, backgroundTaskProgressService)
        {
            MessengerInstance.Register<ExcludeEventPhotosChangedMessage>(this, OnExcludeEventPhotosChanged);
        }

        public override void Cleanup()
        {
            base.Cleanup();
            excludedPhotos = null;
        }

        private void OnExcludeEventPhotosChanged(ExcludeEventPhotosChangedMessage message)
        {
            excludedPhotos = message.Objects;
        }

        protected override void SendPhotoChosenMessage(ICollection<Photo> choosedPhotos, object token)
        {
            MessengerInstance.Send<PhonePhotosChosenMessage>(new PhonePhotosChosenMessage(choosedPhotos));
        }

        protected override ICollection<Photo> GetExcludedPhotos()
        {
            return excludedPhotos == null ? new List<Photo>() : excludedPhotos;
        }
    }
}
