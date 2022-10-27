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

namespace UTraveller.EventMap
{
    public partial class PhotoPushpinDetailsPage : PhoneApplicationPage
    {
        public PhotoPushpinDetailsPage()
        {
            InitializeComponent();
        }

        private void OnPhotoPushpinDeleted(PhotoPushpinDeletedMessage message)
        {
            NavigationService.GoBack();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                App.Messenger.Unregister<PhotoPushpinDeletedMessage>(typeof(PhotoPushpinDetailsPage));
                ((PhotoPushpinDetailsViewModel)DataContext).Cleanup();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                App.Messenger.Register<PhotoPushpinDeletedMessage>(typeof(PhotoPushpinDetailsPage), OnPhotoPushpinDeleted);
                DataContext = App.IocContainer.Get<PhotoPushpinDetailsViewModel>();
                ((PhotoPushpinDetailsViewModel)DataContext).Initialize();
            }
        }
    }
}