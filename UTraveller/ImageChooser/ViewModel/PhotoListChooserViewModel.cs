using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.ViewModel;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.ImageChooser.ViewModel
{
    public abstract class PhotoListChooserViewModel : BasePhotoChooserViewModel
    {
        protected PhotoListViewModel photoListViewModel;

        public PhotoListChooserViewModel(INavigationService navigationService) : base(navigationService)
        {
            this.navigationService = navigationService;
        }


        public PhotoListViewModel PhotoList
        {
            get { return photoListViewModel; }
            set { photoListViewModel = value; }
        }


        protected override void ChoosePhotos()
        {
            var choosedPhotos = new List<Photo>();
            foreach (var image in photoListViewModel)
            {
                if (image.IsChecked)
                {
                    choosedPhotos.Add(image.Photo);
                }
            }
            //parameterContainer.AddParameter("choosedPhotos", choosedPhotos);
            SendPhotoChosenMessage(choosedPhotos, Token);
        }


        public override abstract void Initialize();


        protected override abstract void SendPhotoChosenMessage(ICollection<Photo> choosedPhotos, object token);
    }
}
