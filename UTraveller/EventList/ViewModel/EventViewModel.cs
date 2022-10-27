using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Live;
using Microsoft.Phone.Controls;
using Ninject;
using ServiceApi.UTraveller.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Control;
using UTraveller.Common.Control.Dialog;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventList.ViewModel
{
    public class EventViewModel : BaseViewModel
    {
        private const string EVENT_DETAILS_PAGE = "/EventDetails/EventDetailsPage.xaml";

        private IEventService eventService;
        private INavigationService navigationService;
        private IImageService imageService;
        private IUserService userService;
        private ICancelableTaskProgressService taskProgressService;
        private IParameterContainer<string> parameterContainer;
        private ObservableCollection<EventTileViewModel> events;
        private NotificationService notificationService;
        private ConfirmationService confirmationService;
        private INetworkConnectionCheckService networkConnectionCheckService;
        private ITripBackupService tripBackupService;
        private IMicrosoftLiveAuthService microsoftAuthService;

        public EventViewModel(IEventService eventService, INavigationService navigationService,
            IParameterContainer<string> parameterContainer, IImageService imageService,
            NotificationService notificationService, ConfirmationService confirmationService,
            INetworkConnectionCheckService networkConnectionCheckService, IUserService userService,
            ICancelableTaskProgressService taskProgressService, ITripBackupService tripBackupService,
            IMicrosoftLiveAuthService microsoftAuthService)
        {
            this.eventService = eventService;
            this.navigationService = navigationService;
            this.parameterContainer = parameterContainer;
            this.imageService = imageService;
            this.notificationService = notificationService;
            this.confirmationService = confirmationService;
            this.networkConnectionCheckService = networkConnectionCheckService;
            this.userService = userService;
            this.taskProgressService = taskProgressService;
            this.tripBackupService = tripBackupService;
            this.microsoftAuthService = microsoftAuthService;

            AddEventCommand = new ActionCommand(AddEvent);
            DownloadTripsCommand = new ActionCommand(DownnloadTrips);

            this.events = new ObservableCollection<EventTileViewModel>();

            MessengerInstance.Register<DeletedEventMessage>(this, OnEventDeleted);
            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnShowEventDetails);
            MessengerInstance.Register<EventCurrentChangedMessage>(this, OnCurrentEventChanged);
        }


        public void Initialize()
        {
            var events = eventService.GetEvents(CurrentUser);
            this.events.Clear();
            foreach (var e in events)
            {
                var eventTile = new EventTileViewModel(e);
                imageService.WriteBytesToBitmapImage(e.Image, eventTile.Thumbnail);
                this.events.Add(eventTile);
            }
            UpdateEventCounter();
        }


        public void OpenCurrentEvent()
        {
            Event currentEvent = null;
            if (events != null)
            {
                foreach (var e in events)
                {
                    if (e.Event.IsCurrent)
                    {
                        currentEvent = e.Event;
                        break;
                    }
                }

                if (currentEvent != null)
                {
                    MessengerInstance.Send<EventSelectionChangedMessage>(new EventSelectionChangedMessage(currentEvent));
                }
            }
        }


        public override void Cleanup()
        {
            foreach (var e in events)
            {
                e.Dispose();
            }
            events.Clear();
        }


        public bool HasNoEvents
        {
            get;
            private set;
        }


        public ObservableCollection<EventTileViewModel> Events
        {
            get { return events; }
        }


        public User CurrentUser
        {
            get;
            set;
        }


        public ICommand AddEventCommand
        {
            get;
            private set;
        }


        public ICommand DownloadTripsCommand
        {
            get;
            private set;
        }

        private void AddEvent()
        {
            try
            {
                var e = new Event();
                e.Name = "Trip";
                e.Date = DateTime.Now;
                eventService.AddEvent(e, CurrentUser);

                var eventTile = new EventTileViewModel(e);
                events.Insert(0, eventTile);
                UpdateEventCounter();
            }
            catch (LimitExceedException ex)
            {
                notificationService.Show(string.Format(AppResources.Limit_Exceeded, AppResources.Event, ex.Limit));
            }
        }

        private async void DownnloadTrips()
        {
            if (!networkConnectionCheckService.HasConnection)
            {
                notificationService.Show("You don't have internet connection :(");
            }
            else
            {
                if (!microsoftAuthService.IsSignedIn())
                {
                    await microsoftAuthService.SignIn(App.LiveClientId);
                }
                if (userService.IsAuthenticated() && microsoftAuthService.IsSignedIn())
                {
                    await RunDownloadingTripsTask();
                }
            }
        }

        private async Task RunDownloadingTripsTask()
        {
            try
            {
                var result = await tripBackupService.Restore();
                string warningMesage = string.Format("\nThe following trips were downloaded with warnings: \n {0}", result.WarningMessage);
                if (result.IsSuccess)
                {
                    notificationService.Show("You've successfully downloaded trips from your OneDrive :)" +
                        (result.HasWarnings ? warningMesage : string.Empty));
                }
                else
                {
                    notificationService.Show(string.Format("Sorry, we can't download the following trips from your OneDrive: \n{0}",
                         result.ErrorMessage) + (result.HasWarnings ? warningMesage : string.Empty));
                }
            }
            catch (Exception ex)
            {
                notificationService.Show("Sorry, we can't download trips from your OneDrive because of unexpected error :(");
            }
            finally
            {
                Initialize();
            }
        }


        private void OnShowEventDetails(EventSelectionChangedMessage message)
        {
            navigationService.Navigate(EVENT_DETAILS_PAGE);
        }


        private async void OnEventDeleted(DeletedEventMessage message)
        {
            var e = message.Event;
            var currentUser = userService.GetCurrentUser();
            if (await confirmationService.Show(string.Format("Do you want to delete the trip: {0}", e.Name)))
            {
                taskProgressService.RunIndeterminateProgress(string.Format("Removing trip '{0}'...", e.Name));
                try
                {
                    var isDeleted = eventService.DeleteEvent(e);
                    if (isDeleted)
                    {
                        DeleteEventViewModel(e);
                    }
                    else
                    {
                        notificationService.Show(string.Format("Trip '{0}' was not removed from the Cloud. Please, try later :(", e.Name));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error when deleting trip: " + ex.Message);
                }
                finally
                {
                    taskProgressService.FinishProgress();
                }
            }
        }


        private void OnCurrentEventChanged(EventCurrentChangedMessage message)
        {
            eventService.ChangeCurrentEvent(message.Object);
            foreach (var e in events)
            {
                if (e.Event.Id != message.Object.Id)
                {
                    e.UnsetCurrentStatus();
                }
            }
        }


        private void DeleteEventViewModel(Event e)
        {
            EventTileViewModel eventTileToDelete = null;
            foreach (var eventTile in Events)
            {
                if (eventTile.Event.Id.Equals(e.Id))
                {
                    eventTileToDelete = eventTile;
                    break;
                }
            }
            if (eventTileToDelete != null)
            {
                events.Remove(eventTileToDelete);
                UpdateEventCounter();
            }
        }


        private void UpdateEventCounter()
        {
            HasNoEvents = this.events == null || this.events.Count == 0;
            RaisePropertyChanged("HasNoEvents");
        }
    }
}