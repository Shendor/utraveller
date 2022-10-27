using Microsoft.Expression.Interactivity.Core;
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
using UTraveller.Common.Message;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Sync.ViewModel
{
    public class SyncViewModel : BaseViewModel
    {
        private ISyncService syncService;
        private IEventService eventService;
        private IUserService userService;
        private ITripBackupService tripBackupService;
        private ICancelableTaskProgressService taskProgressService;
        private NotificationService notificationService;
        private Event currentEvent;

        public SyncViewModel(ISyncService syncService, IEventService eventService,
            IUserService userService, ITripBackupService tripBackupService,
            ICancelableTaskProgressService taskProgressService, NotificationService notificationService)
        {
            this.syncService = syncService;
            this.eventService = eventService;
            this.userService = userService;
            this.taskProgressService = taskProgressService;
            this.notificationService = notificationService;
            this.tripBackupService = tripBackupService;

            SyncCommand = new ActionCommand(SyncEntities);
            // UnSyncEntities = new ObservableCollection<SyncEntityViewModel>();

            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);
        }


        public override void Cleanup()
        {
            UnSyncEntities.Clear();
        }


        public void Initialize()
        {
            var unSyncEntitiesViewModel = new List<SyncEntityViewModel>();
            if (!currentEvent.IsSync || currentEvent.IsDeleted)
            {
                unSyncEntitiesViewModel.Add(new SyncEntityViewModel(currentEvent));
            }
            var unSyncEntities = syncService.GetUnSyncEntitiesForEvent(currentEvent);
            foreach (var unSyncEntity in unSyncEntities)
            {
                unSyncEntitiesViewModel.Add(new SyncEntityViewModel(unSyncEntity));
            }
            UnSyncEntities = new List<GroupedSyncItemsViewModel>(GroupSyncItems(unSyncEntitiesViewModel));
            HasNoSyncItems = unSyncEntitiesViewModel.Count == 0;
            RaisePropertyChanged("HasNoSyncItems");
            RaisePropertyChanged("UnSyncEntities");
        }


        public ICommand SyncCommand
        {
            get;
            private set;
        }


        public IList<GroupedSyncItemsViewModel> UnSyncEntities
        {
            get;
            private set;
        }


        public bool HasNoSyncItems
        {
            get;
            private set;
        }


        private IEnumerable<GroupedSyncItemsViewModel> GroupSyncItems(ICollection<SyncEntityViewModel> items)
        {
            var groupedItems =
                from item in items
                group item by item.SyncType into itemForSyncType
                select new GroupedSyncItemsViewModel(itemForSyncType);
            return groupedItems;
        }


        private async void SyncEntities()
        {
            var currentUser = userService.GetCurrentUser();
            if (currentUser.RESTAccessToken != null)
            {
                var entitiesType = new Type[] { typeof(Message), typeof(MoneySpending), typeof(TripPlan), typeof(Photo), typeof(Route) };
                var entitiesName = new string[] { AppResources.Event, AppResources.Message, AppResources.MoneySpendings, AppResources.TripPlan, 
                    AppResources.Photo, AppResources.Route };

                int index = 0;
                try
                {
                    taskProgressService.UpdateProgress(0, entitiesType.Length,
                                                         string.Format(AppResources.Sync_Synchronizing_Progress, entitiesName[index]));
                    //await eventService.SyncEvent(userService.GetCurrentUser(), currentEvent);

                    for (index = 0; index < entitiesType.Length; index++)
                    {
                        taskProgressService.UpdateProgress(index, entitiesType.Length,
                                                             string.Format(AppResources.Sync_Synchronizing_Progress, entitiesName[index + 1]));
                        if (!taskProgressService.IsCanceled)
                        {
                            await syncService.SyncEntitiesForEvent(entitiesType[index], currentEvent);
                        }
                    }
                }
                catch (LimitExceedException lex)
                {
                    notificationService.Show(string.Format(AppResources.Sync_Limit_Exceeded, entitiesName[index + 1], lex.Limit));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error in sync entities: " + ex.Message);
                }
                finally
                {
                    taskProgressService.FinishProgress();
                    Cleanup();
                    Initialize();
                    if (UnSyncEntities.Count > 0)
                    {
                        notificationService.Show(AppResources.Sync_Fail);
                    }
                    else
                    {
                        notificationService.Show(AppResources.Sync_Success);
                    }
                }
            }
            else
            {
                notificationService.Show(AppResources.Sync_Not_Connected);
            }
        }


        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            currentEvent = message.Object;
        }


        public void ShowDescription(BaseModel baseModel)
        {
            if (baseModel is Event)
            {
                notificationService.Show(((Event)baseModel).Name);
            }
            else if (baseModel is Photo)
            {
                var photo = (Photo)baseModel;
                if (photo.RemoteId > 0 && !photo.IsSync && photo.ImageUrl == null)
                {
                    notificationService.Show("This photo is in the Cloud but its image is not in your 'utraveler photos private album' in Facebook. Please, sign in to Facebook, enable the option to save images in this album and sync you photos again.");
                }
                else if (!photo.IsSync && photo.ImageUrl != null)
                {
                    notificationService.Show("Caption or position in map was changed");
                }
                else if (photo.RemoteId <= 0)
                {
                    notificationService.Show("New photo");
                }
            }
            else if (baseModel is Message)
            {
                notificationService.Show(((Message)baseModel).Text);
            }
            else if (baseModel is MoneySpending)
            {
                var ms = (MoneySpending)baseModel;
                notificationService.Show(string.Format("{0} - {1} {2}",
                    MoneySpendingCategoryUtil.GetMoneySpendingCategoryName(ms.MoneySpendingCategory),
                    ms.Amount, CurrencyUtil.GetCurrencyName(ms.Currency)));
            }
            else if (baseModel is Route)
            {
                notificationService.Show(((Route)baseModel).Name);
            }
        }
    }
}
