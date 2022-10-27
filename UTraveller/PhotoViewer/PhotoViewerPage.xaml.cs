using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using UTraveller.Common.Control;
using UTraveller.Common.Message;
using UTraveller.PhotoViewer.Control;
using UTraveller.PhotoViewer.Messages;
using UTraveller.PhotoViewer.ViewModel;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.PhotoViewer
{
    public partial class PhotoViewerPage : BasePhoneApplicationPage
    {
        private IImageInitializer imageInitializer;
        private DetailedTimelineItemViewModel previousTimeLineItemViewModel;
        private PhotoViewerViewModel viewModel;
        private PhotoItemControl selectedPhotoItem;
        private INetworkConnectionCheckService networkConnectionCheckService;
        private int timeLineItemPosition = -1;
        private bool isAddingItemsInProgress;

        public PhotoViewerPage()
        {
            InitializeComponent();

            imageInitializer = App.IocContainer.Get<IImageInitializer>("mediaLibraryPhotoImageInitializer");
            networkConnectionCheckService = App.IocContainer.Get<INetworkConnectionCheckService>();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                eventItemsList.Items.Clear();
                App.Messenger.Unregister<NetworkStatusChangedMessage>(typeof(PhotoViewerPage));
                App.Messenger.Unregister<TimeLineItemPostedMessage>(typeof(PhotoViewerPage));
                if (viewModel != null)
                {
                    viewModel.Cleanup();
                }
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                viewModel = App.IocContainer.Get<PhotoViewerViewModel>();
                if (viewModel.TimeLineItems == null || viewModel.TimeLineItems.Count == 0)
                {
                    viewModel.ShowNoContentDialog();
                    NavigationService.GoBack();
                }
                else
                {
                    DataContext = viewModel;
                    App.Messenger.Register<NetworkStatusChangedMessage>(typeof(PhotoViewerPage), OnNetworkStatusChanged);
                    App.Messenger.Register<TimeLineItemPostedMessage>(typeof(PhotoViewerPage), OnTimeLineItemPosted);

                    viewModel.Initialize();

                    if (viewModel.TimeLineItems != null)
                    {
                        AddPhotos();
                    }
                }
            }
        }


        private void AddPhotos()
        {
            //System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            //watch.Start();
            if (viewModel.TimeLineItems.Count > 0)
            {
                isAddingItemsInProgress = true;
                DetailedTimelineItemViewModel previousViewModel = null;

                int selectedIndex = 0;
                foreach (var timeLineItem in viewModel.TimeLineItems)
                {
                    if (eventItemsList.Items.Count > 0)
                    {
                        eventItemsList.Items.Add(CreateTimeLineFlipViewItem(timeLineItem));
                        break;
                    }
                    if (timeLineItem.IsSelected)
                    {
                        timeLineItemPosition = timeLineItem.PositionNumber;
                        if (previousViewModel != null)
                        {
                            eventItemsList.Items.Add(CreateTimeLineFlipViewItem(previousViewModel));
                        }
                        eventItemsList.Items.Add(CreateTimeLineFlipViewItem(timeLineItem));
                        selectedIndex = eventItemsList.Items.Count - 1;
                    }

                    previousViewModel = timeLineItem;
                }
                isAddingItemsInProgress = false;
                eventItemsList.SelectedIndex = selectedIndex;
            }
            //watch.Stop();
            //System.Diagnostics.Debug.WriteLine(watch.ElapsedMilliseconds);
        }


        private FlipViewItem CreateTimeLineFlipViewItem(DetailedTimelineItemViewModel timeLineViewModel)
        {
            FlipViewItem pivotItem;
            if (timeLineViewModel.DateItem is Photo)
            {
                // imageInitializer.InitializeImage(timeLineViewModel.DateItem as Photo);
                pivotItem = new PhotoItemControl();
            }
            else
            {
                pivotItem = new MessageItemControl();
            }
            pivotItem.DataContext = timeLineViewModel;
            return pivotItem;
        }


        private void OnTimeLineItemPosted(TimeLineItemPostedMessage message)
        {
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = !message.Object;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).IsEnabled = message.Object;
        }


        private void OnNetworkStatusChanged(NetworkStatusChangedMessage message)
        {
            var isEnabled = message.Object && !viewModel.IsPosted;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled = isEnabled;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).IsEnabled = isEnabled;
        }


        private async void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((!isAddingItemsInProgress || timeLineItemPosition == 0) && e.AddedItems.Count > 0 && e.AddedItems[0] is FlipViewItem)
            {

                if (e.AddedItems[0] is PhotoItemControl)
                {
                    selectedPhotoItem = e.AddedItems[0] as PhotoItemControl;
                    previousTimeLineItemViewModel = selectedPhotoItem.DataContext as DetailedTimelineItemViewModel;
                    var photo = (previousTimeLineItemViewModel = (selectedPhotoItem.DataContext as DetailedTimelineItemViewModel)).DateItem as Photo;

                    if (photo.ImageStream == null || photo.Image.PixelWidth == 0)
                    {
                        var isInitialized = await imageInitializer.InitializeImage(photo);
                        if (!isInitialized && photo.ImageUrl == null)
                        {
                            selectedPhotoItem.FailInitialization();
                        }
                        else
                        {
                            selectedPhotoItem.InitializeImage(Orientation, !isInitialized && photo.ImageUrl != null);
                        }
                    }
                    else
                    {
                        selectedPhotoItem.InitializeImage(Orientation, false);
                    }

                    viewModel.SelectTimelineItem(previousTimeLineItemViewModel);
                }
                else if (e.AddedItems[0] is MessageItemControl)
                {
                    selectedPhotoItem = null;
                    var messageItem = e.AddedItems[0] as MessageItemControl;
                    previousTimeLineItemViewModel = messageItem.DataContext as DetailedTimelineItemViewModel;
                    viewModel.SelectTimelineItem(previousTimeLineItemViewModel);
                }
                if (timeLineItemPosition >= 0)
                {
                    AddTimeLineItemAfterSelectionChanged();
                    timeLineItemPosition = previousTimeLineItemViewModel.PositionNumber;
                }
                ChangeFacebookMenuItemsAvailability();
                ChangeSocialInfoPanelVisibility();
                InitializePhotoDescriptionVisibility();
            }
        }


        private void ChangeFacebookMenuItemsAvailability()
        {
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[1]).IsEnabled =
                networkConnectionCheckService.HasConnection && !viewModel.IsPosted && viewModel.IsConnectedToFacebook;
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[2]).IsEnabled =
                networkConnectionCheckService.HasConnection && viewModel.IsPosted && viewModel.IsConnectedToFacebook;
        }


        private void AddTimeLineItemAfterSelectionChanged()
        {
            // Add timeline item to the right side
            if (previousTimeLineItemViewModel.PositionNumber < viewModel.TimeLineItems.Count &&
                previousTimeLineItemViewModel.PositionNumber > timeLineItemPosition)
            {
                AddTimeLineItemToSideAfterSelectionChanged(1);
            }
            // Add timeline item to the left side
            else if (previousTimeLineItemViewModel.PositionNumber > 0 &&
                previousTimeLineItemViewModel.PositionNumber < timeLineItemPosition)
            {
                AddTimeLineItemToSideAfterSelectionChanged(-1);
            }
        }


        private void AddTimeLineItemToSideAfterSelectionChanged(int side)
        {
            foreach (var timeLineItem in viewModel.TimeLineItems)
            {
                if (timeLineItem.PositionNumber == previousTimeLineItemViewModel.PositionNumber + side)
                {
                    if (side > 0 && (eventItemsList.SelectedIndex == eventItemsList.Items.Count - 1
                                 && previousTimeLineItemViewModel.PositionNumber < viewModel.TimeLineItems.Count))
                    {
                        eventItemsList.Items.Add(CreateTimeLineFlipViewItem(timeLineItem));

                        if (eventItemsList.Items.Count > 30)
                        {
                            var flipViewItem = eventItemsList.Items[eventItemsList.Items.Count - 25] as FlipViewItem;
                            DisposePhoto(((DetailedTimelineItemViewModel)flipViewItem.DataContext).DateItem as Photo);
                            eventItemsList.Items.RemoveAt(0);
                        }
                    }
                    else if (side < 0 && (eventItemsList.SelectedIndex == 0 && previousTimeLineItemViewModel.PositionNumber > 0))
                    {
                        eventItemsList.Items.Insert(0, CreateTimeLineFlipViewItem(timeLineItem));
                        if (eventItemsList.Items.Count > 30)
                        {
                            var flipViewItem = eventItemsList.Items[eventItemsList.Items.Count - 1] as FlipViewItem;
                            DisposePhoto(((DetailedTimelineItemViewModel)flipViewItem.DataContext).DateItem as Photo);
                            eventItemsList.Items.RemoveAt(eventItemsList.Items.Count - 1);
                        }
                    }

                    break;
                }
            }
        }

        private void DisposePhoto(Photo photo)
        {
            if (photo != null)
            {
                imageInitializer.DisposeImage(photo);
            }
        }

        private void InitializePhoto(Photo photo)
        {
            if (photo != null)
            {
                imageInitializer.InitializeImage(photo);
            }
        }


        private void PostFacebookButtonTap(object sender, EventArgs e)
        {
            if (viewModel.IsPhotoSelected())
            {
                postPanel.Visibility = Visibility.Visible;
            }
            else if (viewModel.IsMessageSelected())
            {
                postMessagePanel.Visibility = Visibility.Visible;
            }
        }


        private void CancelPostButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ClosePostPanels();
        }


        private void OkPostButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ClosePostPanels();
        }


        private void CloseCommentsButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            facebookCommentsPanel.Visibility = Visibility.Collapsed;
        }


        private void FacebookCommentsButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            facebookCommentsPanel.Visibility = Visibility.Visible;
        }


        private void ViewCommentsAndLikesTap(object sender, EventArgs e)
        {
            facebookCommentsPanel.Visibility = Visibility.Visible;
        }


        private void PageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (selectedPhotoItem != null)
            {
                selectedPhotoItem.ChangePageOrientation(Orientation);
                if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.LandscapeLeft ||
                    Orientation == PageOrientation.LandscapeRight)
                {
                    ApplicationBar.Mode = ApplicationBarMode.Minimized;
                    ApplicationBar.Opacity = 0;
                }
                else
                {
                    ApplicationBar.Opacity = 0.5;
                }
            }
        }


        private void EventItemsListTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (ApplicationBar.IsVisible)
            {
                dateLabel.Visibility = Visibility.Collapsed;
                ApplicationBar.IsVisible = false;
            }
            else
            {
                dateLabel.Visibility = Visibility.Visible;
                ApplicationBar.IsVisible = true;
            }
            InitializePhotoDescriptionVisibility();
            ChangeSocialInfoPanelVisibility();
        }


        private void InitializePhotoDescriptionVisibility()
        {
            if (selectedPhotoItem != null)
            {
                selectedPhotoItem.DescriptionVisibility(ApplicationBar.IsVisible);
            }
        }


        private void ChangeSocialInfoPanelVisibility()
        {
            if (viewModel.IsPosted && viewModel.IsConnectedToFacebook && ApplicationBar.IsVisible)
            {
                socialInfoPanel.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                socialInfoPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }


        private void WriteDescriptionButtonTap(object sender, EventArgs e)
        {
            viewModel.WriteDescription();
        }


        private void ClosePostPanels()
        {
            postPanel.Visibility = Visibility.Collapsed;
            postMessagePanel.Visibility = Visibility.Collapsed;
        }

    }
}