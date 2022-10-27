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
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using Microsoft.Phone.Maps.Toolkit;
using UTraveller.EventMap.Messages;
using UTravellerModel.UTraveller.Model;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Media.Animation;
using UTraveller.Common.Message;
using UTraveller.Service.Api;
using System.Windows.Media.Imaging;
using UTraveller.Common.Util;
using System.Threading.Tasks;
using UTraveller.Common.Control;
using Windows.System;
using UTraveller.EventMap.ViewModel.Map;

namespace UTraveller.Map
{
    public partial class EventMapPage : BasePhoneApplicationPage
    {
        private static readonly int DEFAULT_PHOTOS_PANEL_WIDTH = 125;
        private static readonly int DEFAULT_PLAN_ITEMS_LEGEND_PANEL_WIDTH = 220;
        private static readonly BitmapImage SHOW_PIN_ICON = new BitmapImage(new Uri("/Assets/Icons/show_pin.png", UriKind.Relative));
        private static readonly BitmapImage HIDE_PIN_ICON = new BitmapImage(new Uri("/Assets/Icons/hide_pin.png", UriKind.Relative));

        private bool isEditMode;
        private bool isPushpinsVisible = true;
        private Pushpin selectedPushpin;
        private FrameworkElement routePushpinPanel;
        private EventMapViewModel viewModel;
        private IRouteFileReader routeFileReader;
        private IList<MapElement> cacheMapElements = new List<MapElement>();

        public EventMapPage()
        {
            InitializeComponent();
            this.ApplicationBar = (IApplicationBar)Resources["mainAppBar"];
            routeFileReader = App.IocContainer.Get<IRouteFileReader>();
        }


        private void MapHold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (selectedPushpin == null && isEditMode)
            {
                mapHoldEllipseStoryboard.Stop();

                var originCooordinate = e.GetPosition(map);
                var geoCoordinate = map.ConvertViewportPointToGeoCoordinate(originCooordinate);
                viewModel.AddPushpin(geoCoordinate);
                map.SetView(geoCoordinate, map.ZoomLevel);
            }
        }


        private void MapCenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            if (mapHoldEllipseStoryboard.GetCurrentState() != ClockState.Stopped)
            {
                mapHoldEllipseStoryboard.Stop();
            }

            if (selectedPushpin != null && isEditMode)
                selectedPushpin.GeoCoordinate = map.Center;
        }


        private void MapMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (routePushpinPanel != null)
            {
                routePushpinPanel.Visibility = System.Windows.Visibility.Collapsed;
            }
            // Check OriginalSource is Grid - fast solution to check if tap on map. May not work if map will be
            // wrapped by other containers
            if (mapHoldEllipseStoryboard.GetCurrentState() == ClockState.Stopped && e.OriginalSource is Grid)
            {
                var originCooordinate = e.GetPosition(map);
                var geoCoordinate = map.ConvertViewportPointToGeoCoordinate(originCooordinate);

                ((MapOverlay)map.Layers[3][0]).GeoCoordinate = geoCoordinate;

                mapHoldEllipseStoryboard.Begin();
            }
        }


        private void MapManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            if (mapHoldEllipseStoryboard.GetCurrentState() != ClockState.Stopped)
            {
                mapHoldEllipseStoryboard.Stop();
            }
        }


        protected async override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                App.Messenger.Register<PushpinFoundMessage>(typeof(EventMapPage), OnPushpinFound);
                App.Messenger.Register<RouteChosenMessage>(typeof(EventMapPage), OnRouteChosenFound);

                viewModel = App.IocContainer.Get<EventMapViewModel>();
                DataContext = viewModel;
                viewModel.Initialize();

                await SetMapViewByCoordinates(viewModel.GetPushpinsCoordinates());
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                App.Messenger.Unregister<PushpinFoundMessage>(typeof(EventMapPage));
                App.Messenger.Unregister<RouteChosenMessage>(typeof(EventMapPage));
                viewModel.Cleanup();
                map.MapElements.Clear();
                map.Layers.Clear();
                cacheMapElements.Clear();
                map = null;
            }
        }


        private void OnPushpinFound(PushpinFoundMessage message)
        {
            map.SetView(message.Object.Coordinate, map.ZoomLevel);
        }


        private void OnRouteChosenFound(RouteChosenMessage message)
        {
            map.MapElements.Clear();
            var coordinates = viewModel.GetPushpinsCoordinates();
            foreach (var route in message.Objects)
            {
                foreach (var routeCoordinates in route.Coordinates)
                {
                    MapPolyline routeLine = new MapPolyline();
                    routeLine.StrokeColor = route.Color;
                    routeLine.StrokeThickness = 5.0;
                    routeLine.Path = GeoCoordinateUtils.FromGeoCoordinates(routeCoordinates.Coordinates);
                    if (routeLine.Path.Count > 2)
                    {
                        coordinates.Add(routeLine.Path[routeLine.Path.Count / 2]);
                    }
                    map.MapElements.Add(routeLine);
                }

                foreach (var polygon in route.Polygons)
                {
                    var mapPolygon = new MapPolygon();
                    mapPolygon.FillColor = polygon.Color;
                    mapPolygon.StrokeThickness = 2;
                    mapPolygon.StrokeColor = Color.FromArgb(255, polygon.Color.R, polygon.Color.G, polygon.Color.B);
                    mapPolygon.Path = new GeoCoordinateCollection();
                    for (int i = 0; i < polygon.Coordinates.Count - 2; i += 2)
                    {
                        mapPolygon.Path.Add(new GeoCoordinate(polygon.Coordinates[i], polygon.Coordinates[i + 1]));
                    }
                    map.MapElements.Add(mapPolygon);
                }
            }
            SetMapViewByCoordinates(coordinates);
        }


        private void PushPinTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (isEditMode)
            {
                if (selectedPushpin == sender)
                {
                    viewModel.ChangePushpinCoordinate(selectedPushpin.Tag as TimeLineItemPushpinViewModel, selectedPushpin.GeoCoordinate);
                    selectedPushpin = null;
                }
                else
                {
                    selectedPushpin = (Pushpin)sender;
                    map.SetView(selectedPushpin.GeoCoordinate, map.ZoomLevel);
                }
            }
            else
            {
                var photoPushpin = ((Pushpin)sender).Tag as TimeLineItemPushpinViewModel;
                App.Messenger.Send<ViewPushpinPhotosMessage>(new ViewPushpinPhotosMessage(photoPushpin));
            }
        }


        private void RoutePushPinTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!isEditMode)
            {
                var routePushpin = ((Pushpin)sender).Tag as RoutePushpinViewModel;

                if (routePushpin != null)
                {
                    map.Center = routePushpin.Pushpin.Coordinate;
                    viewModel.ShowPushpinDescription(routePushpin.Pushpin.Description, routePushpin.Pushpin.Coordinate);
                }
            }
        }


        private void PlanItemPushPinTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!isEditMode)
            {
                var planItemPushpin = ((Pushpin)sender).Tag as PlanItemPushpinViewModel;

                if (planItemPushpin != null)
                {
                    map.Center = planItemPushpin.BasePlanItem.Coordinate;
                    var description = !string.IsNullOrEmpty(planItemPushpin.BasePlanItem.Description) ?
                        string.Format("\"{0}\"\n{1}", planItemPushpin.BasePlanItem.Description, planItemPushpin.BasePlanItem.Address) :
                        planItemPushpin.BasePlanItem.Address;
                    viewModel.ShowPushpinDescription(description, planItemPushpin.BasePlanItem.Coordinate);
                }
            }
        }


        private void RoutesVisibilityCheckBoxTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            if (checkBox.IsChecked.Value)
            {
                foreach (var mapElement in cacheMapElements)
                {
                    map.MapElements.Add(mapElement);
                }
                cacheMapElements.Clear();
            }
            else
            {
                foreach (var mapElement in map.MapElements)
                {
                    cacheMapElements.Add(mapElement);
                }
                map.MapElements.Clear();
            }

        }


        private void ShowPhotosPanelButtonTap(object sender, EventArgs e)
        {
            if (photosPanel.Width == DEFAULT_PHOTOS_PANEL_WIDTH)
            {
                photosOpacityAnimation.From = 1;
                photosOpacityAnimation.To = 0;
                photosOpacityAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, 40);

                showPhotosPanelAnimation.From = DEFAULT_PHOTOS_PANEL_WIDTH;
                showPhotosPanelAnimation.To = 0;
                showPhotosPanelAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, 0);
                showPhotosPanelStoryboard.Begin();
            }
            else if (photosPanel.Width == 0)
            {
                photosOpacityAnimation.From = 0;
                photosOpacityAnimation.To = 1;
                photosOpacityAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, 0);

                showPhotosPanelAnimation.From = 0;
                showPhotosPanelAnimation.To = DEFAULT_PHOTOS_PANEL_WIDTH;
                showPhotosPanelAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, 40);
                showPhotosPanelStoryboard.Begin();
            }
        }


        private void ShowPlanItemsLegendPanelButtonTap(object sender, EventArgs e)
        {
            if (planItemsLegendPanel.Width == DEFAULT_PLAN_ITEMS_LEGEND_PANEL_WIDTH)
            {
                planItemsLegendOpacityAnimation.From = 1;
                planItemsLegendOpacityAnimation.To = 0;
                planItemsLegendOpacityAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, 40);

                showPlanItemsLegendPanelAnimation.From = DEFAULT_PLAN_ITEMS_LEGEND_PANEL_WIDTH;
                showPlanItemsLegendPanelAnimation.To = 0;
                showPlanItemsLegendPanelAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, 0);
                showPlanItemsLegendPanelStoryboard.Begin();
            }
            else if (planItemsLegendPanel.Width == 0)
            {
                planItemsLegendOpacityAnimation.From = 0;
                planItemsLegendOpacityAnimation.To = 1;
                planItemsLegendOpacityAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, 0);

                showPlanItemsLegendPanelAnimation.From = 0;
                showPlanItemsLegendPanelAnimation.To = DEFAULT_PLAN_ITEMS_LEGEND_PANEL_WIDTH;
                showPlanItemsLegendPanelAnimation.BeginTime = new TimeSpan(0, 0, 0, 0, 40);
                showPlanItemsLegendPanelStoryboard.Begin();
            }
        }


        private void ApplyEditButtonTap(object sender, EventArgs e)
        {
            isEditMode = false;
            this.ApplicationBar = (IApplicationBar)Resources["mainAppBar"];
            mapBorder.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }


        private void EditModeMenuItemTap(object sender, EventArgs e)
        {
            isEditMode = true;
            this.ApplicationBar = (IApplicationBar)Resources["editAppBar"];
            mapBorder.BorderBrush = viewModel.MainColor;
        }


        private void RoutesMenuItemTap(object sender, EventArgs e)
        {
            viewModel.ShowRoutesList();
        }


        private void MyLocationButtonTap(object sender, EventArgs e)
        {
            viewModel.ShowMyLocation(MyCurrentLocationChanged);
        }


        private void MyCurrentLocationChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            currentLocationEllipseStoryboard.Stop();
            var mapLayer = ((MapOverlay)map.Layers[4][0]);
            mapLayer.GeoCoordinate = e.Position.Location;
            map.SetView(e.Position.Location, map.ZoomLevel);
            ((Grid)mapLayer.Content).Visibility = System.Windows.Visibility.Visible;
            currentLocationEllipseStoryboard.Begin();
        }


        private void MapOptionButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (mapOptionStoryboard.GetCurrentState() != ClockState.Active)
            {
                if (mapOptionButtonAnimation1.From == 0 && mapOptionStoryboard.GetCurrentTime().Milliseconds > 1)
                {
                    mapOptionButtonAnimation1.From = 50;
                    mapOptionButtonAnimation1.To = 0;
                    mapOptionButtonAnimation1.BeginTime = TimeSpan.Zero;

                    mapOptionLineAnimation1.From = 40;
                    mapOptionLineAnimation1.To = 0;
                    mapOptionLineAnimation1.BeginTime = TimeSpan.FromSeconds(0.2);

                    mapPanelButtonAnimation2.From = mapFilterCheckBox2.IsChecked.Value ? 50 : 0;
                    mapPanelButtonAnimation2.To = 0;
                    mapPanelButtonAnimation2.BeginTime = TimeSpan.Zero;

                    mapPanelLineAnimation2.From = mapFilterCheckBox2.IsChecked.Value ? 40 : 0;
                    mapPanelLineAnimation2.To = 0;
                    mapPanelLineAnimation2.BeginTime = TimeSpan.FromSeconds(0.2);

                    mapPanelButtonAnimation1.From = mapFilterCheckBox1.IsChecked.Value ? 50 : 0;
                    mapPanelButtonAnimation1.To = 0;
                    mapPanelButtonAnimation1.BeginTime = TimeSpan.Zero;

                    mapPanelLineAnimation1.From = mapFilterCheckBox1.IsChecked.Value ? 40 : 0;
                    mapPanelLineAnimation1.To = 0;
                    mapPanelLineAnimation1.BeginTime = TimeSpan.FromSeconds(0.2);

                    mapFilterCheckBoxAnimation3.From = 50;
                    mapFilterCheckBoxAnimation3.To = 0;
                    mapFilterCheckBoxAnimation3.BeginTime = TimeSpan.FromSeconds(0.3);

                    mapFilterLineAnimation3.From = 40;
                    mapFilterLineAnimation3.To = 0;
                    mapFilterLineAnimation3.BeginTime = TimeSpan.FromSeconds(0.5);

                    mapFilterCheckBoxAnimation2.From = 50;
                    mapFilterCheckBoxAnimation2.To = 0;
                    mapFilterCheckBoxAnimation2.BeginTime = TimeSpan.FromSeconds(0.6);

                    mapFilterLineAnimation2.From = 40;
                    mapFilterLineAnimation2.To = 0;
                    mapFilterLineAnimation2.BeginTime = TimeSpan.FromSeconds(0.8);

                    mapFilterCheckBoxAnimation1.From = 50;
                    mapFilterCheckBoxAnimation1.To = 0;
                    mapFilterCheckBoxAnimation1.BeginTime = TimeSpan.FromSeconds(0.9);

                    mapFilterLineAnimation1.From = 40;
                    mapFilterLineAnimation1.To = 0;
                    mapFilterLineAnimation1.BeginTime = TimeSpan.FromSeconds(1.1);
                }
                else
                {
                    mapFilterLineAnimation1.From = 0;
                    mapFilterLineAnimation1.To = 40;
                    mapFilterLineAnimation1.BeginTime = TimeSpan.Zero;

                    mapFilterCheckBoxAnimation1.From = 0;
                    mapFilterCheckBoxAnimation1.To = 50;
                    mapFilterCheckBoxAnimation1.BeginTime = TimeSpan.FromSeconds(0.2);

                    mapFilterLineAnimation2.From = 0;
                    mapFilterLineAnimation2.To = 40;
                    mapFilterLineAnimation2.BeginTime = TimeSpan.FromSeconds(0.3);

                    mapFilterCheckBoxAnimation2.From = 0;
                    mapFilterCheckBoxAnimation2.To = 50;
                    mapFilterCheckBoxAnimation2.BeginTime = TimeSpan.FromSeconds(0.5);

                    mapFilterLineAnimation3.From = 0;
                    mapFilterLineAnimation3.To = 40;
                    mapFilterLineAnimation3.BeginTime = TimeSpan.FromSeconds(0.6);

                    mapFilterCheckBoxAnimation3.From = 0;
                    mapFilterCheckBoxAnimation3.To = 50;
                    mapFilterCheckBoxAnimation3.BeginTime = TimeSpan.FromSeconds(0.8);

                    mapOptionLineAnimation1.From = 0;
                    mapOptionLineAnimation1.To = 40;
                    mapOptionLineAnimation1.BeginTime = TimeSpan.FromSeconds(0.9);

                    mapOptionButtonAnimation1.From = 0;
                    mapOptionButtonAnimation1.To = 50;
                    mapOptionButtonAnimation1.BeginTime = TimeSpan.FromSeconds(1.1);

                    mapPanelLineAnimation1.From = 0;
                    mapPanelLineAnimation1.To = mapFilterCheckBox1.IsChecked.Value ? 40 : 0;
                    mapPanelLineAnimation1.BeginTime = TimeSpan.FromSeconds(0.9);

                    mapPanelButtonAnimation1.From = 0;
                    mapPanelButtonAnimation1.To = mapFilterCheckBox1.IsChecked.Value ? 50 : 0;
                    mapPanelButtonAnimation1.BeginTime = TimeSpan.FromSeconds(1.1);

                    mapPanelLineAnimation2.From = 0;
                    mapPanelLineAnimation2.To = mapFilterCheckBox2.IsChecked.Value ? 40 : 0;
                    mapPanelLineAnimation2.BeginTime = TimeSpan.FromSeconds(0.9);

                    mapPanelButtonAnimation2.From = 0;
                    mapPanelButtonAnimation2.To = mapFilterCheckBox2.IsChecked.Value ? 50 : 0;
                    mapPanelButtonAnimation2.BeginTime = TimeSpan.FromSeconds(1.1);
                }
                mapOptionStoryboard.Begin();
            }
        }


        private void FilterCheckBoxTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (sender.Equals(mapFilterCheckBox1))
            {
                if (mapFilterCheckBox1.IsChecked.Value)
                {
                    panelLineAnimation1.From = 0;
                    panelLineAnimation1.To = 40;
                    panelLineAnimation1.BeginTime = TimeSpan.Zero;

                    panelButtonAnimation1.From = 0;
                    panelButtonAnimation1.To = 50;
                    panelButtonAnimation1.BeginTime = TimeSpan.FromSeconds(0.2);
                }
                else
                {
                    panelButtonAnimation1.From = 50;
                    panelButtonAnimation1.To = 0;
                    panelButtonAnimation1.BeginTime = TimeSpan.Zero;

                    panelLineAnimation1.From = 40;
                    panelLineAnimation1.To = 0;
                    panelLineAnimation1.BeginTime = TimeSpan.FromSeconds(0.2);
                }
                panelButtonsStoryboard1.Begin();
            }
            else if (sender.Equals(mapFilterCheckBox2))
            {
                if (mapFilterCheckBox2.IsChecked.Value)
                {
                    panelLineAnimation2.From = 0;
                    panelLineAnimation2.To = 40;
                    panelLineAnimation2.BeginTime = TimeSpan.Zero;

                    panelButtonAnimation2.From = 0;
                    panelButtonAnimation2.To = 50;
                    panelButtonAnimation2.BeginTime = TimeSpan.FromSeconds(0.2);
                }
                else
                {
                    panelButtonAnimation2.From = 50;
                    panelButtonAnimation2.To = 0;
                    panelButtonAnimation2.BeginTime = TimeSpan.Zero;

                    panelLineAnimation2.From = 40;
                    panelLineAnimation2.To = 0;
                    panelLineAnimation2.BeginTime = TimeSpan.FromSeconds(0.2);
                }
                panelButtonsStoryboard2.Begin();
            }
        }


        private void PageOrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if (e.Orientation == PageOrientation.Landscape || e.Orientation == PageOrientation.LandscapeLeft ||
                e.Orientation == PageOrientation.LandscapeRight)
            {
                mapToolbarPanel.Margin = new Thickness(70, 0, 0, 0);
            }
            else
            {
                mapToolbarPanel.Margin = new Thickness(0, 0, 0, 0);
            }
        }


        private async Task SetMapViewByCoordinates(ICollection<GeoCoordinate> coordinates)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                if (coordinates != null && coordinates.Count > 1)
                {
                    var area = GeoCoordinateUtils.GetArea(coordinates);
                    Thread.Sleep(1000); //Hack, but in other case view is not set by area properly
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (map != null)
                        {
                            map.SetView(area, MapAnimationKind.Linear);
                        }
                    });
                }
                else if (coordinates != null && coordinates.Count == 1)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (map != null)
                        {
                            map.SetView(coordinates.First(), 10, MapAnimationKind.Linear);
                        }
                    });
                }
            });
        }


        private void HelpMenuItemTap(object sender, EventArgs e)
        {
            viewModel.ShowMapHelp();
        }


        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "40cc0963-cca2-4d40-a1e5-7cb998f80c74";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "25NpDJTnsb7oi72SBh23Nw";
        }

    }
}