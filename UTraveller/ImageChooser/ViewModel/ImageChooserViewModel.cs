using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using Ninject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.ImageChooser.Model;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.ImageChooser.ViewModel
{
    public abstract class ImageChooserViewModel : BasePhotoChooserViewModel
    {
        private ICollection<ImageListViewModel> imagesCollection;
        private IPhotoUploader photoUploader;
        private ITaskProgressService backgroundTaskProgressService;
        private IDictionary<string, ICollection<Photo>> photosInAlbums;

        public ImageChooserViewModel(IPhotoUploader photoUploader,
            INavigationService navigationService,
            [Named("backgroundTaskProgressService")] ITaskProgressService backgroundTaskProgressService)
            : base(navigationService)
        {
            this.photoUploader = photoUploader;
            this.backgroundTaskProgressService = backgroundTaskProgressService;
        }

        public override void Initialize()
        {
            UploadPhotos();
        }

        public override void Cleanup()
        {
            photosInAlbums = null;
            imagesCollection = null;
        }

        public bool HasPhotos()
        {
            return HasGroupedPhotoThumbnails;
        }

        public bool HasGroupedPhotoThumbnails
        {
            get { return ImagesCollection.Count > 0; }
        }


        public ICollection<ImageListViewModel> ImagesCollection
        {
            get { return imagesCollection; }
        }

        public void InitializePhotosInAlbum(ImageListViewModel album)
        {
            if (album != null && album.Images == null)
            {
                var photos = WrapToCheckedPhotoViewModel(photosInAlbums[album.FolderName]);
                album.Images = new List<GroupedImagesViewModel>(GroupPictures(photos));
            }
        }

        private async void UploadPhotos()
        {
            imagesCollection = new ObservableCollection<ImageListViewModel>();
            backgroundTaskProgressService.RunIndeterminateProgress();
            photosInAlbums = await photoUploader.LoadPhotos();

            foreach (var album in photosInAlbums)
            {
                try
                {
                    var imageListViewModel = new ImageListViewModel();
                    imageListViewModel.FolderName = album.Key;
                    if (imagesCollection.Count == 0)
                    {
                        imageListViewModel.Images = new List<GroupedImagesViewModel>(GroupPictures(WrapToCheckedPhotoViewModel(album.Value)));
                    }
                    imagesCollection.Add(imageListViewModel);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error uploading photos: " + ex.Message);
                }
            }
            backgroundTaskProgressService.FinishProgress();
        }


        private ICollection<CheckedImageModel> WrapToCheckedPhotoViewModel(ICollection<Photo> photos)
        {
            var photoViewModels = new HashSet<CheckedImageModel>();
            foreach (var photo in photos)
            {
                if (GetExcludedPhotos().Where(p => p.Name.Equals(photo.Name) &&
                    p.Date.TimeOfDay.Seconds == photo.Date.TimeOfDay.Seconds).Count() == 0)
                {
                    photoViewModels.Add(new CheckedImageModel(photo));
                }
            }
            return photoViewModels;
        }

        private IEnumerable<GroupedImagesViewModel> GroupPictures(ICollection<CheckedImageModel> pictures)
        {
            var groupedPhotos =
                from photo in pictures
                orderby photo.Photo.Date
                group photo by photo.Photo.Date.ToString("y") into photosByMonth
                select new GroupedImagesViewModel(photosByMonth, navigationService, MessengerInstance);
            return groupedPhotos;
        }

        protected override void ChoosePhotos()
        {
            var choosedPhotos = new List<Photo>();

            foreach (var imagesItem in imagesCollection)
            {
                if (imagesItem.Images != null)
                {
                    foreach (var image in imagesItem.Images)
                    {
                        foreach (var item in image)
                        {
                            if (item.IsChecked)
                            {
                                choosedPhotos.Add(item.Photo);
                            }
                        }
                    }
                }
            }

            SendPhotoChosenMessage(choosedPhotos, Token);
        }


        protected abstract ICollection<Photo> GetExcludedPhotos();


        protected override abstract void SendPhotoChosenMessage(ICollection<Photo> choosedPhotos, object token);

    }
}
