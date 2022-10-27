using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using Ninject;
using ServiceApi.UTraveller.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UTraveller.Common;
using UTraveller.Common.Control;
using UTraveller.Common.Control.Dialog;
using UTraveller.Common.Message;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using System.IO;
using UTraveller.Service.Model;
using UTraveller.Routes.Messages;

namespace UTraveller.Routes.ViewModel
{
    public class RoutesViewModel : BaseViewModel, IFileOpenPickerContinue
    {
        private static readonly string SKY_DRIVE_PAGE = "/SkyDrive/SkyDriveExplorerPage.xaml";

        private ICommand addRouteCommand;
        private ICommand chooseRoutesCommand;
        private ICollection<RouteViewModel> routes;
        private IRouteService routeService;
        private INavigationService navigationService;
        private ITaskProgressService progressService;
        private ITaskProgressService backgroundTaskProgressService;
        private IRouteFileReader routeFileReader;
        private INetworkConnectionCheckService networkConnectionCheckService;
        private IZipExtractService zipExtractService;
        private NotificationService notificationService;
        private ConfirmationService confirmationService;
        private Event currentEvent;
        private bool hasNoRoutes;

        public RoutesViewModel(IRouteService routeService, INavigationService navigationService,
            ICancelableTaskProgressService progressService,
            [Named("backgroundTaskProgressService")] ITaskProgressService backgroundTaskProgressService,
            NotificationService notificationService, ConfirmationService confirmationService,
            IRouteFileReader routeFileReader, INetworkConnectionCheckService networkConnectionCheckService,
            IZipExtractService zipExtractService)
        {
            this.routeService = routeService;
            this.navigationService = navigationService;
            this.progressService = progressService;
            this.notificationService = notificationService;
            this.backgroundTaskProgressService = backgroundTaskProgressService;
            this.confirmationService = confirmationService;
            this.routeFileReader = routeFileReader;
            this.networkConnectionCheckService = networkConnectionCheckService;
            this.zipExtractService = zipExtractService;
            routes = new ObservableCollection<RouteViewModel>();

            addRouteCommand = new ActionCommand(AddRoute);
            AddRouteFromOneDriveCommand = new ActionCommand(AddRouteUsingOneDrive);
            chooseRoutesCommand = new ActionCommand(ChooseRoutes);

            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);
            MessengerInstance.Register<RouteAddedMessage>(this, OnRouteAdded);
            MessengerInstance.Register<RouteDeletedMessage>(this, OnRouteDeleted);
            MessengerInstance.Register<ViewRouteDescriptionMessage>(this, OnViewRouteDescription);
        }

        public string LiveClientId
        {
            get { return App.LiveClientId; }
        }


        public ICollection<RouteViewModel> Routes
        {
            get { return routes; }
        }

        public override void Cleanup()
        {
            Routes.Clear();
        }

        public void Initialize()
        {
            backgroundTaskProgressService.RunIndeterminateProgress();

            try
            {
                var routes = routeService.GetRoutes(currentEvent);
                if (routes != null)
                {
                    Routes.Clear();
                    foreach (var route in routes)
                    {
                        Routes.Add(new RouteViewModel(route));
                    }
                }
                UpdateHasNotRoutesFlag();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in reading of routes: " + ex.Message);
            }
            finally
            {
                backgroundTaskProgressService.FinishProgress();
            }
        }


        public bool HasNoRoutes
        {
            get { return hasNoRoutes; }
            set
            {
                hasNoRoutes = value;
                RaisePropertyChanged("HasNoRoutes");
            }
        }


        #region Commands
        public ICommand AddRouteCommand
        {
            get { return addRouteCommand; }
        }

        public ICommand AddRouteFromOneDriveCommand
        {
            get;
            private set;
        }

        public ICommand ChooseRoutesCommand
        {
            get { return chooseRoutesCommand; }
        }
        #endregion

        #region Command Handlers
        private void AddRoute()
        {
            if (currentEvent != null)
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".kml");
                openPicker.FileTypeFilter.Add(".kmz");

                openPicker.PickSingleFileAndContinue();
            }
        }


        private void AddRouteUsingOneDrive()
        {
            if (currentEvent != null)
            {
                navigationService.Navigate(SKY_DRIVE_PAGE, this.GetType().ToString());
            }
        }


        private void ChooseRoutes()
        {
            var chosenRoutes = new List<Route>();
            foreach (var route in Routes)
            {
                if (route.IsSelected)
                {
                    routeService.InitializeRouteData(route.Route);
                    chosenRoutes.Add(route.Route);
                }
            }
            MessengerInstance.Send<RouteChosenMessage>(new RouteChosenMessage(chosenRoutes));
        }
        #endregion

        #region Event Handler
        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            currentEvent = message.Object;
        }


        private void OnRouteAdded(RouteAddedMessage message)
        {
            if (message.Object == null)
            {
                progressService.FinishProgress();
                notificationService.Show("Can't read this route file :(");
                return;
            }
            try
            {
                var route = new Route();
                route.Name = message.RouteInfo.Name;
                route.Description = message.RouteInfo.Description;
                route.Coordinates = message.RouteInfo.Coordinates;
                route.Polygons = message.RouteInfo.Polygons;
                route.Pushpins = message.RouteInfo.Pushpins;
                route.Color = ColorGenerator.GetRandomColor(Routes.Count + 1);
                routeService.AddRoute(route, currentEvent);

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    Routes.Add(new RouteViewModel(route));
                    UpdateHasNotRoutesFlag();
                    notificationService.Show("You have successfully added route: " + route.Name); //TODO: i18n
                });
            }
            catch (LimitExceedException lex)
            {
                notificationService.Show(string.Format(AppResources.Limit_Exceeded, AppResources.Route, lex.Limit));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in adding route: " + ex.Message);
            }
            finally
            {
                progressService.FinishProgress();
            }
        }


        private async void OnRouteDeleted(RouteDeletedMessage message)
        {
            if (await confirmationService.Show("Do you want to delete this item?"))
            {
                routeService.DeleteRoute(message.Object, currentEvent);
                var routeViewModelToDelete = Routes.First((vm) => { return vm.Route.Id == message.Object.Id; });
                if (routeViewModelToDelete != null)
                {
                    Routes.Remove(routeViewModelToDelete);
                    UpdateHasNotRoutesFlag();
                }
            }
        }


        private void OnViewRouteDescription(ViewRouteDescriptionMessage message)
        {
            notificationService.Show(message.Description);
        }


        private void UpdateHasNotRoutesFlag()
        {
            HasNoRoutes = Routes.Count == 0;
        }


        public async void ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs args)
        {
            bool isContinue = args.Files.Count > 0;
            if (isContinue && !networkConnectionCheckService.HasConnection)
            {
                isContinue = await confirmationService.Show("Warning! \nYou don't have an internet connection thus Placemarks from this Route file will have Default icons. " +
                    "Do you want to continue?");
            }
            if (isContinue)
            {
                progressService.RunIndeterminateProgress("Reading route...");
                try
                {
                    foreach (StorageFile file in args.Files)
                    {
                        IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.Read);
                        if (fileStream != null)
                        {
                            var stream = file.FileType.Equals("." + FileExtensionType.KMZ.ToString().ToLower()) ?
                                await zipExtractService.Unzip(fileStream.AsStream()) : fileStream.AsStream();
                            var routeInfo = await routeFileReader.ReadRoute(stream);
                            MessengerInstance.Send<RouteAddedMessage>(new RouteAddedMessage(routeInfo));

                        }
                    }
                }
                catch (Exception ex)
                {
                    progressService.FinishProgress();
                    notificationService.Show("Can't read the route file because of unexpected error :(");
                }
            }
        }
        #endregion
    }
}
