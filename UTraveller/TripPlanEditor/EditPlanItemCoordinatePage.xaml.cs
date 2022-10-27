using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Toolkit;
using UTraveller.TripPlanEditor.ViewModel;
using Ninject;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using System.Threading.Tasks;
using System.Threading;
using UTraveller.Common.Control;

namespace UTraveller.TripPlanEditor
{
    public partial class EditPlanItemCoordinatePage : BasePhoneApplicationPage
    {
        private static double mapZoom = 14;

        private EditPlanItemCoordinateViewModel viewModel;
        private bool isEdit;

        public EditPlanItemCoordinatePage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                viewModel = App.IocContainer.Get<EditPlanItemCoordinateViewModel>();
                DataContext = viewModel;
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back && viewModel != null)
            {
                mapZoom = map.ZoomLevel;
                viewModel.Cleanup();
                map.MapElements.Clear();
                map.Layers.Clear();
            }
        }


        private void ApplyEditButtonTap(object sender, EventArgs e)
        {
            viewModel.ApplyCoordinate(pushpin.GeoCoordinate);
            NavigationService.GoBack();
        }


        private void MapCenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            if (isEdit)
            {
                pushpin.GeoCoordinate = map.Center;
            }
        }


        private void PushpinTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (viewModel.IsViewMode)
            {
                viewModel.ShowAddress();
            }
            else
            {
                isEdit = !isEdit;
            }
        }


        private void CloseButtonTap(object sender, EventArgs e)
        {
            Close();
        }


        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            if (viewModel != null)
            {
                isEdit = viewModel.IsEditAvailableFirstTime;
                ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = !viewModel.IsViewMode;
                if (viewModel.Coordinate != null)
                {
                    map.SetView(viewModel.Coordinate, mapZoom, MapAnimationKind.None);
                }
            }
        }

    }
}