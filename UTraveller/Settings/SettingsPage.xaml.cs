using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Shell;
using UTraveller.Settings.ViewModel;
using Ninject;
using Microsoft.Live;
using UTraveller.Common.Message;
using Microsoft.Live.Controls;
using Microsoft.Phone.Tasks;
using UTraveller.Service.Api;
using Microsoft.Phone.Controls;
using UTraveller.Common.Control;

namespace UTraveller.Settings
{
    public partial class SettingsPage : BasePhoneApplicationPage
    {
        private SettingsViewModel viewModel;

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                viewModel = App.IocContainer.Get<SettingsViewModel>();
                viewModel.Initialize();
                DataContext = viewModel;
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                viewModel.Cleanup();
            }
        }


        private void BackgroundButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            viewModel.ChangeBackgroundColor();
        }


        private void MainColorButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            viewModel.ChangeMainColorColor();
        }


        private void TextColorButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            viewModel.ChangeTextColorColor();
        }


        private void SignInSkyDriveSessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                App.Messenger.Send<SkyDriveSessionChangedMessage>(new SkyDriveSessionChangedMessage(e.Session));
            }
            else
            {
                App.Messenger.Send<SkyDriveSessionChangedMessage>(new SkyDriveSessionChangedMessage(null));
            }
        }

        private void OneDriveHelpButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            oneDriveHelpPanel.Visibility = oneDriveHelpPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }


        private void FacebookHelpButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            facebookHelpPanel.Visibility = facebookHelpPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }


        private void UploadFacebookButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            uploadFacebookLabel.Visibility = uploadFacebookLabel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ConnectToServerHelpButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            connectToServerHelpPanel.Visibility = connectToServerHelpPanel.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }


        private void ChangeAvatarTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowImageChooserTaskAndApplyChange((s, evnt) =>
            {
                if (evnt.TaskResult == TaskResult.OK)
                {
                    viewModel.ShowChangeAvatarPage(evnt.ChosenPhoto);
                }
            });
        }


        private void ChangeCoverTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowImageChooserTaskAndApplyChange((s, evnt) =>
            {
                if (evnt.TaskResult == TaskResult.OK)
                {
                    var width = App.Current.Host.Content.ActualWidth;
                    var height = App.Current.Host.Content.ActualHeight;
                    var delimiter = App.AppProperties.IsLandscapeCover ? 2.2 : 1.4;
                    viewModel.ShowChangeCoverPage(evnt.ChosenPhoto, (int)width, (int)(height / delimiter));
                }
            });
        }


        private void ShowImageChooserTaskAndApplyChange(EventHandler<PhotoResult> handler)
        {
            var photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += handler;
            photoChooserTask.Show();
        }


        private async void ConnectFacebookTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (viewModel.IsConnectedToFacebook)
            {
                var webBrowser = new WebBrowser();
                await webBrowser.ClearCookiesAsync();
                await webBrowser.ClearInternetCacheAsync();

                var logoutUrl = viewModel.GetLogoutUrl();
                viewModel.LogoutFacebook();
                webBrowser.Navigate(logoutUrl);
            }
            else
            {
                viewModel.ConnectFacebook();
            }
        }
    }
}