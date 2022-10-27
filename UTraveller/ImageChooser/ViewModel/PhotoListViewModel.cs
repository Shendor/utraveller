using GalaSoft.MvvmLight.Messaging;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.ImageChooser.Model;
using UTraveller.Service.Api;

namespace UTraveller.ImageChooser.ViewModel
{
    public class PhotoListViewModel : BaseListViewModel<CheckedImageModel>
    {
        private static readonly string IMAGE_LIST_GALLERY_URI = "/ImageChooser/Control/DetailedImageChooser.xaml";

        protected ICommand showDetailedImagesCommand;
        protected ICommand checkAllThumbnailsCommand;
        protected INavigationService navigationService;
        protected IMessenger messenger;

        public PhotoListViewModel(IGrouping<string, CheckedImageModel> grouping, INavigationService navigationService,
            IMessenger messenger)
            : base(grouping)
        {
            this.messenger = messenger;
            Initialize(navigationService);
        }

        public PhotoListViewModel(INavigationService navigationService)
        {
            Initialize(navigationService);
        }

        public ICommand ShowDetailedImagesCommand
        {
            get { return showDetailedImagesCommand; }
        }

        public ICommand CheckAllThumbnailsCommand
        {
            get { return checkAllThumbnailsCommand; }
        }

        private void ShowDetailedImages()
        {
            messenger.Send<ObjectsCollectionMessage<CheckedImageModel>>(new ObjectsCollectionMessage<CheckedImageModel>(this));
            navigationService.Navigate(IMAGE_LIST_GALLERY_URI);
        }

        private void CheckAllThumbnails(object isChecked)
        {
            if (isChecked is bool)
            {
                foreach (var thumbnail in this)
                {
                    thumbnail.IsChecked = (bool)isChecked;
                }
            }
        }

        private void Initialize(INavigationService navigationService)
        {
            this.navigationService = navigationService;

            showDetailedImagesCommand = new ActionCommand(ShowDetailedImages);
            checkAllThumbnailsCommand = new ActionCommand(CheckAllThumbnails);
        }
    }
}

