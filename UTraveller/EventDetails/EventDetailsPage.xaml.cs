using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using UTraveller.EventDetails.ViewModel;
using Microsoft.Live;
using Microsoft.Live.Controls;
using UTraveller.Service.Implementation;
using UTraveller.Service.Api;
using System.Diagnostics;
using UTraveller.Common.Message;
using UTraveller.Resources;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using System.IO;
using Windows.Foundation;
using System.Runtime.InteropServices.WindowsRuntime;
using UTravellerModel.UTraveller.Model;
using System.Windows.Media.Animation;
using System.Windows;
using UTraveller.Common.Control;
using System.IO.IsolatedStorage;
using System.Windows.Media;

namespace UTraveller.ImageViewer.Control
{
    public partial class EventDetailsPage : BasePhoneApplicationPage
    {
        private static readonly Uri GRID_ICON = new Uri("/Assets/Icons/grid.png", UriKind.Relative);
        private static readonly Uri LIST_ICON = new Uri("/Assets/Icons/list.png", UriKind.Relative);
        private static readonly Uri VIEW_ICON = new Uri("/Assets/Icons/view.png", UriKind.Relative);
        private static readonly Uri EDIT_ICON = new Uri("/Assets/Icons/pen.png", UriKind.Relative);
        private static readonly ImageBrush VIEW_IMAGE = new ImageBrush() { ImageSource = new BitmapImage(VIEW_ICON) };
        private static readonly ImageBrush EDIT_IMAGE = new ImageBrush() { ImageSource = new BitmapImage(EDIT_ICON) };
        private static readonly string ITEMS_VIEW_KEY = "ITEMS_VIEW_KEY";
        private static readonly string IS_EDIT_TOOLBAR_KEY = "EVENT_EDIT_TOOLBAR";

        private System.Windows.Point initialpoint;
        private EventDetailsViewModel viewModel;

        public EventDetailsPage()
        {
            InitializeComponent();
            InitializeDefaultItemsView();
            InitializeDefaultToolbar();
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                viewModel = App.IocContainer.Get<EventDetailsViewModel>();
                DataContext = viewModel;
            }
            viewModel.Initialize();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode == NavigationMode.Back && viewModel != null)
            {
                viewModel.Cleanup();
                DataContext = null;
                viewModel = null;
            }
        }


        private void CameraButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var shareMedia = new ShareMediaTask();
            var cameraCaptureTask = new CameraCaptureTask();

            cameraCaptureTask.Completed += (s, result) =>
            {
                if (result.TaskResult == TaskResult.OK)
                {
                    viewModel.AddPhoto(result.OriginalFileName);
                }
            };
            cameraCaptureTask.Show();
        }


        private void PhotoTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var tag = ((FrameworkElement)sender).Tag;
            if (tag is IDateItem)
            {
                viewModel.ViewTimelineItems(tag as IDateItem);
            }
        }


        private void ToolbarButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SwapToolbar((Button)sender);
        }


        private void SwapToolbar(Button button)
        {
            if (toolbarStoryboard.GetCurrentState() != ClockState.Active)
            {
                if (timeLineItemButton3.Height >= 49)
                {
                    IsolatedStorageSettings.ApplicationSettings[IS_EDIT_TOOLBAR_KEY] = true;
                    SwapToEditToolbar(button);
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings[IS_EDIT_TOOLBAR_KEY] = false;
                    SwapToViewToolbar(button);
                }
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }


        private void Grid_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            initialpoint = e.ManipulationOrigin;
        }


        private void EventImageTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            viewModel.ChangeEventImage();
        }


        private void ItemsViewButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            try
            {
                if (timeLineItemsPanel.Visibility == Visibility.Visible)
                {
                    SetListTimeLineView();
                }
                else
                {
                    SetGridTimeLineView();
                }
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            catch (AccessViolationException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void SetGridTimeLineView()
        {
            itemsViewImage.ImageSource.SetValue(BitmapImage.UriSourceProperty, GRID_ICON);
            timeLineItemsPanel.Visibility = Visibility.Visible;
            groupedTimeLineItemsPanel.Visibility = Visibility.Collapsed;
            IsolatedStorageSettings.ApplicationSettings[ITEMS_VIEW_KEY] = false;
        }


        private void SetListTimeLineView()
        {
            itemsViewImage.ImageSource.SetValue(BitmapImage.UriSourceProperty, LIST_ICON);
            timeLineItemsPanel.Visibility = Visibility.Collapsed;
            groupedTimeLineItemsPanel.Visibility = Visibility.Visible;
            IsolatedStorageSettings.ApplicationSettings[ITEMS_VIEW_KEY] = true;
        }


        private void BackButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close();
        }


        private void InitializeDefaultItemsView()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(ITEMS_VIEW_KEY))
            {
                bool isListView = (bool)IsolatedStorageSettings.ApplicationSettings[ITEMS_VIEW_KEY];
                if (isListView)
                {
                    SetListTimeLineView();
                }
                else
                {
                    SetGridTimeLineView();
                }
            }
        }


        private void InitializeDefaultToolbar()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(IS_EDIT_TOOLBAR_KEY))
            {
                bool isEditToolbar = (bool)IsolatedStorageSettings.ApplicationSettings[IS_EDIT_TOOLBAR_KEY];
                if (isEditToolbar)
                {
                    SwapToEditToolbar(toolbarSwitchButton);
                }
                else
                {
                    SwapToViewToolbar(toolbarSwitchButton);
                }
            }
        }


        private void PlanButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            planControl.Visibility = Visibility.Visible;
            expenseControl.Visibility = Visibility.Collapsed;
            viewModel.TripPlanViewMode = TripPlanViewMode.Plan;
        }


        private void ExpenseButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            planControl.Visibility = Visibility.Collapsed;
            expenseControl.Visibility = Visibility.Visible;
            viewModel.TripPlanViewMode = TripPlanViewMode.Expense;
        }


        private void SwapToViewToolbar(Button button)
        {
            button.Content = VIEW_IMAGE;

            itemsViewButtonAnimation.From = 50;
            itemsViewButtonAnimation.To = 0;
            itemsViewButtonAnimation.BeginTime = TimeSpan.Zero;

            mapLine2Animation.From = 40;
            mapLine2Animation.To = 0;
            mapLine2Animation.BeginTime = TimeSpan.FromSeconds(0.2);

            mapButtonAnimation.From = 50;
            mapButtonAnimation.To = 0;
            mapButtonAnimation.BeginTime = TimeSpan.FromSeconds(0.3);

            mapLineAnimation.From = 40;
            mapLineAnimation.To = 0;
            mapLineAnimation.BeginTime = TimeSpan.FromSeconds(0.5);

            timeLineItemLineAnimation1.From = 0;
            timeLineItemLineAnimation1.To = 40;
            timeLineItemLineAnimation1.BeginTime = TimeSpan.FromSeconds(0.6);

            timeLineButtonLineAnimation1.From = 0;
            timeLineButtonLineAnimation1.To = 50;
            timeLineButtonLineAnimation1.BeginTime = TimeSpan.FromSeconds(0.8);

            timeLineItemLineAnimation2.From = 0;
            timeLineItemLineAnimation2.To = 40;
            timeLineItemLineAnimation2.BeginTime = TimeSpan.FromSeconds(0.9);

            timeLineButtonLineAnimation2.From = 0;
            timeLineButtonLineAnimation2.To = 50;
            timeLineButtonLineAnimation2.BeginTime = TimeSpan.FromSeconds(1.1);

            timeLineItemLineAnimation3.From = 0;
            timeLineItemLineAnimation3.To = 40;
            timeLineItemLineAnimation3.BeginTime = TimeSpan.FromSeconds(1.2);

            timeLineButtonLineAnimation3.From = 0;
            timeLineButtonLineAnimation3.To = 50;
            timeLineButtonLineAnimation3.BeginTime = TimeSpan.FromSeconds(1.4);

            toolbarStoryboard.Begin();
        }

        private void SwapToEditToolbar(Button button)
        {
            button.Content = EDIT_IMAGE;

            itemsViewButtonAnimation.From = 0;
            itemsViewButtonAnimation.To = 50;
            itemsViewButtonAnimation.BeginTime = TimeSpan.FromSeconds(1.5);

            mapLine2Animation.From = 0;
            mapLine2Animation.To = 40;
            mapLine2Animation.BeginTime = TimeSpan.FromSeconds(1.3);

            mapButtonAnimation.From = 0;
            mapButtonAnimation.To = 50;
            mapButtonAnimation.BeginTime = TimeSpan.FromSeconds(1.2);

            mapLineAnimation.From = 0;
            mapLineAnimation.To = 40;
            mapLineAnimation.BeginTime = TimeSpan.FromSeconds(1);

            timeLineItemLineAnimation1.From = 40;
            timeLineItemLineAnimation1.To = 0;
            timeLineItemLineAnimation1.BeginTime = TimeSpan.FromSeconds(0.9);

            timeLineButtonLineAnimation1.From = 50;
            timeLineButtonLineAnimation1.To = 0;
            timeLineButtonLineAnimation1.BeginTime = TimeSpan.FromSeconds(0.7);

            timeLineItemLineAnimation2.From = 40;
            timeLineItemLineAnimation2.To = 0;
            timeLineItemLineAnimation2.BeginTime = TimeSpan.FromSeconds(0.6);

            timeLineButtonLineAnimation2.From = 50;
            timeLineButtonLineAnimation2.To = 0;
            timeLineButtonLineAnimation2.BeginTime = TimeSpan.FromSeconds(0.4);

            timeLineItemLineAnimation3.From = 40;
            timeLineItemLineAnimation3.To = 0;
            timeLineItemLineAnimation3.BeginTime = TimeSpan.FromSeconds(0.2);

            timeLineButtonLineAnimation3.From = 50;
            timeLineButtonLineAnimation3.To = 0;
            timeLineButtonLineAnimation3.BeginTime = TimeSpan.Zero;

            toolbarStoryboard.Begin();
        }

        private void ChangeDateRangeTextBlockTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            viewModel.EditDateRange();
        }

    }
}