using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.Resources;
using Microsoft.Xna.Framework.Media;
using System.Collections.ObjectModel;
using System.Device.Location;
using Microsoft.Phone.Tasks;
using System.IO;
using System.Windows.Media.Imaging;
using Ninject;
using UTraveller.Service.Api;
using UTraveller.EventList.ViewModel;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using UTraveller.Common.Control;
using System.Threading;
using UTraveller.PhotoCrop.Message;
using UTraveller.Service.Implementation;
using System.Windows.Media.Animation;
using System.Threading.Tasks;
using Microsoft.Live;
using UTraveller.Common.Message;
using Microsoft.Live.Controls;
using System.IO.IsolatedStorage;

namespace UTraveller
{
    public partial class MainPage : BasePhoneApplicationPage
    {
        private System.Windows.Point initialpoint;
        private Popup popup;
        private UserProfileViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();
            InitializeSwipeHelpPanel();
            ShowSplash();
        }


        public override void Activate()
        {
            base.Activate();
            if (viewModel != null && DataContext == null)
            {
                StartLoadingData();
            }
            else if (viewModel != null && DataContext != null)
            {
                viewModel.Initialize();
            }
        }


        public override void Deactivate()
        {
            base.Deactivate();
            if (viewModel != null)
            {
                viewModel.Cleanup();
            }
        }


        private void ShowSplash()
        {
            if (this.popup == null)
            {
                this.popup = new Popup();
            }
            var splashScreen = new SplashScreen();
            splashScreen.Width = Application.Current.Host.Content.ActualWidth;
            splashScreen.Height = Application.Current.Host.Content.ActualHeight;
            this.popup.Child = splashScreen;
            this.popup.IsOpen = true;
            StartLoadingData();
        }


        private async void StartLoadingData()
        {
            viewModel = App.IocContainer.Get<UserProfileViewModel>();
            var hasUser = await viewModel.TryAuthenticateCurrentUser();
            if (!hasUser)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    DataContext = viewModel;
                    viewModel.ShowWalkthrough();
                    this.popup.IsOpen = false;
                });
            }
            else
            {
                viewModel.Initialize();
                DataContext = viewModel;
                this.popup.IsOpen = false;
                this.popup.Child = null;

                InitializeCoverHeight();
                Dispatcher.BeginInvoke(() =>
                {
                    viewModel.TryOpenCurrentEvent();
                });
                viewModel.CheckIfVersionIsUpToDate();
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.New && viewModel != null)
            {
                viewModel.Cleanup();
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode == NavigationMode.Back && viewModel != null)
            {
                if (DataContext == null)
                {
                    DataContext = viewModel;
                }
                viewModel.Initialize();
                InitializeCoverHeight();
            }
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
                    viewModel.ShowChangeCoverPage(evnt.ChosenPhoto, (int)coverBorder.ActualWidth, (int)coverBorder.ActualHeight);
                }
            });
        }


        private void ShowImageChooserTaskAndApplyChange(EventHandler<PhotoResult> handler)
        {
            var photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += handler;
            photoChooserTask.Show();
        }


        private void Grid_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            initialpoint = e.ManipulationOrigin;
        }


        private void InitializeCoverHeight()
        {
            if (!App.AppProperties.IsLandscapeCover)
            {
                coverHeight.Height = new GridLength(App.Current.Host.Content.ActualHeight - 320, GridUnitType.Star);
            }
            else
            {
                coverHeight.Height = new GridLength(App.Current.Host.Content.ActualHeight / 2 - 100, GridUnitType.Pixel);
            }
        }

        private void InitializeSwipeHelpPanel()
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains(App.IS_LAUNCHED_FIRST_TIME))
            {
                swipeHelpPanel.Visibility = System.Windows.Visibility.Visible;
                IsolatedStorageSettings.ApplicationSettings.Add(App.IS_LAUNCHED_FIRST_TIME, false);
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        private void ChangeToProfileViewButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            mainPivot.SelectedIndex = 0;
        }


        private void SignInMicrosoftAccountSessionChanged(object sender, LiveConnectSessionChangedEventArgs e)
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
    }
}