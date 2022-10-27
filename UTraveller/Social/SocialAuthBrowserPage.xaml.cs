using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Diagnostics;
using UTraveller.Social.ViewModel;
using Ninject;

namespace UTraveller.Social
{
    public partial class SocialAuthBrowserPage : PhoneApplicationPage
    {
        private SocialAuthViewModel viewModel;

        public SocialAuthBrowserPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                viewModel.Cleanup();
            }
        }


        protected async override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                viewModel = App.IocContainer.Get<SocialAuthViewModel>();
                viewModel.Token = NavigationContext.QueryString[UTraveller.Service.Implementation.NavigationService.PARAMETER_NAME]; 
                DataContext = viewModel;
                await webBrowser.ClearCookiesAsync();
                await webBrowser.ClearInternetCacheAsync();

                var url = viewModel.CreateAuthUri();
                webBrowser.Navigated += BrowserNavigated;
                webBrowser.Navigate(url);
            }
        }


        private void BrowserNavigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            try
            {
                if (viewModel.Login(e.Uri))
                {
                    webBrowser.Navigated -= BrowserNavigated;
                    viewModel.Cleanup();
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}