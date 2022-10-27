using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.UI.Input;
using Ninject;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;
using GalaSoft.MvvmLight;
using UTraveller.ImageChooser.ViewModel;
using UTraveller.ImageChooser.Model;

namespace UTraveller.PhotoViewer.Control
{
    public partial class DetailedImageChooser : PhoneApplicationPage
    {
        private IImageInitializer imageInitializer;
        private Photo previousPhoto;

        public DetailedImageChooser()
        {
            InitializeComponent();
        }

        private void PhotosSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is CheckedImageModel)
            {
                if (previousPhoto != null)
                {
                    imageInitializer.DisposeImage(previousPhoto);
                }
                imageInitializer.InitializeImage(previousPhoto = (e.AddedItems[0] as CheckedImageModel).Photo);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                imageInitializer = App.IocContainer.Get<IImageInitializer>("mediaLibraryPhotoImageInitializer");

                DataContext = App.IocContainer.Get<DetailedImageChooserViewModel>();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ((ViewModelBase)DataContext).Cleanup();
            }
        }
    }
}