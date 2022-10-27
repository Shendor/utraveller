using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.ImageChooser.Model;

namespace UTraveller.ImageChooser.ViewModel
{
    public class DetailedImageChooserViewModel : BaseViewModel
    {
        private ICollection<CheckedImageModel> images;

        public DetailedImageChooserViewModel()
        {
            MessengerInstance.Register<ObjectsCollectionMessage<CheckedImageModel>>(this, OnImagesChanged);
        }

        private void OnImagesChanged(ObjectsCollectionMessage<CheckedImageModel> message)
        {
            images = message.Objects;
        }

        public override void Cleanup()
        {
            images = null;
        }

        public ICollection<CheckedImageModel> Images
        {
            get { return images; }
        }
    }
}
