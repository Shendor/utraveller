using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections;
using System.Windows.Input;
using Ninject;
using GalaSoft.MvvmLight.Messaging;
using UTraveller.Common.Message;
using GalaSoft.MvvmLight;
using UTraveller.EventDetails.ViewModel;
using UTraveller.ImageChooser.ViewModel;
using UTraveller.Common;
using UTraveller.Service.Api;
using UTraveller.Common.Control;

namespace UTraveller.PhotoViewer.Control
{
    public partial class ImageChooser : BasePhoneApplicationPage
    {
        private ImageChooserViewModel viewModel;

        public ImageChooser()
        {
            InitializeComponent();
        }

        private void OnAddedPhotos(ChoosedPhotosMessage message)
        {
            NavigationService.GoBack();
        }

        private void PhotosInDetailsTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = true;
        }

        private void SelectAllPhotosTapped(object sender, System.Windows.Input.GestureEventArgs e)
        {
            e.Handled = true;
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                var viewModel = DataContext as ImageChooserViewModel;
                if (viewModel != null)
                {
                    App.Messenger.Unregister<PhonePhotosChosenMessage>(typeof(ImageChooser));
                    viewModel.Cleanup();
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                IDictionary<string, ViewModelBase> viewModels = CreateViewModelsDictionary();
                viewModel =
                    viewModels[NavigationContext.QueryString[UTraveller.Service.Implementation.NavigationService.PARAMETER_NAME]] as ImageChooserViewModel;

                if (viewModel != null && DataContext != viewModel)
                {
                    App.Messenger.Register<PhonePhotosChosenMessage>(typeof(ImageChooser), OnAddedPhotos);

                    viewModel.Initialize();
                    DataContext = viewModel;
                }
            }
        }


        private void ImagesListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ImageListViewModel)
            {
                viewModel.InitializePhotosInAlbum(e.AddedItems[0] as ImageListViewModel);
            }
        }


        private static IDictionary<string, ViewModelBase> CreateViewModelsDictionary()
        {
            IDictionary<string, ViewModelBase> viewModels = new Dictionary<string, ViewModelBase>();
            viewModels.Add(typeof(EventDetailsViewModel).ToString(), App.IocContainer.Get<PhonePhotoGroupedListChooserViewModel>());
            return viewModels;
        }

    }
}
