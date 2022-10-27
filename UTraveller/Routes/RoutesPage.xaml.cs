using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.Common.Message;
using UTraveller.Routes.ViewModel;
using Ninject;
using Microsoft.Live.Controls;
using Microsoft.Live;
using System.Diagnostics;
using UTraveller.Common;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using UTraveller.Service.Api;
using UTraveller.Common.Control;

namespace UTraveller.Routes
{
    public partial class RoutesPage : BasePhoneApplicationPage, IFilePickerContinuationViewModelPage
    {
        private RoutesViewModel viewModel;

        public RoutesPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                App.Messenger.Register<RouteChosenMessage>(typeof(RoutesPage), OnRouteChosen);

                viewModel = App.IocContainer.Get<RoutesViewModel>();
                DataContext = viewModel;
            }
            viewModel.Initialize();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                App.Messenger.Unregister<RoutesViewModel>(typeof(RoutesViewModel));
                viewModel.Cleanup();
            }
        }


        private void SignInSkyDriveSessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            addButton.IsEnabled = false;
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                App.Messenger.Send<SkyDriveSessionChangedMessage>(new SkyDriveSessionChangedMessage(e.Session));
                addButton.IsEnabled = true;
            }
            else
            {
                App.Messenger.Send<SkyDriveSessionChangedMessage>(new SkyDriveSessionChangedMessage(null));
            }
        }


        private void OnRouteChosen(RouteChosenMessage message)
        {
            Close();
        }


        public IFileOpenPickerContinue ViewModel
        {
            get { return viewModel; }
        }
    }
}