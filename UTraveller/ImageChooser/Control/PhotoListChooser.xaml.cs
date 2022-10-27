using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using UTraveller.EventMap.ViewModel;
using UTraveller.Common.Message;
using GalaSoft.MvvmLight;
using UTraveller.ImageChooser.ViewModel;
using UTraveller.Service.Api;
using UTraveller.EventDetails.ViewModel;

namespace UTraveller.ImageChooser.Control
{
    public partial class PhotoListChooser : PhoneApplicationPage
    {
        public PhotoListChooser()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            ClosePage();
        }

        private void OnAddedPhotos(EventPhotosChosenMessage message)
        {
            ClosePage();
        }

        private void ClosePage()
        {
            NavigationService.GoBack();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);

            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                var viewModels = CreateViewModelsDictionary();
                var parameter = NavigationContext.QueryString[UTraveller.Service.Implementation.NavigationService.PARAMETER_NAME];
                PhotoListChooserViewModel viewModel = viewModels[parameter] as PhotoListChooserViewModel;

                if (viewModel != null)
                {
                    viewModel.Token = parameter;
                    App.Messenger.Register<EventPhotosChosenMessage>(typeof(PhotoListChooser), viewModel.Token, OnAddedPhotos);

                    viewModel.Initialize();
                    DataContext = viewModel;
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                var viewModel = DataContext as ViewModelBase;
                if (viewModel != null)
                {
                    App.Messenger.Unregister<EventPhotosChosenMessage>(typeof(PhotoListChooser));
                    viewModel.Cleanup();
                }
            }
        }

        private static IDictionary<string, PhotoListChooserViewModel> CreateViewModelsDictionary()
        {
            IDictionary<string, PhotoListChooserViewModel> viewModels = new Dictionary<string, PhotoListChooserViewModel>();
            viewModels.Add(typeof(EventMapViewModel).ToString(), App.IocContainer.Get<EventPhotoListChooserViewModel>());
            viewModels.Add(typeof(EventDetailsViewModel).ToString(), App.IocContainer.Get<EventPhotoListChooserViewModel>());
            return viewModels;
        }
    }
}