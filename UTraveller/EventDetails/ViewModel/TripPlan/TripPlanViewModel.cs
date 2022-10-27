using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTraveller.TripPlanEditor.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class TripPlanViewModel : BaseViewModel
    {
        private static readonly string EDIT_PLAN_ITEM_PATH = "/TripPlanEditor/EditPlanItemPage.xaml";

        private ITripPlanService tripPlanService;
        private INavigationService navigationService;
        private Event currentEvent;
        private TripPlan tripPlan;

        public TripPlanViewModel(ITripPlanService tripPlanService, INavigationService navigationService)
        {
            this.tripPlanService = tripPlanService;
            this.navigationService = navigationService;
            PlanItems = new List<GroupedPlanItemViewModel>();

            MessengerInstance.Register<PlanItemSavedMessage>(this, OnPlanItemSaved);
            MessengerInstance.Register<PlanItemDeletedMessage>(this, OnPlanItemDeleted);
            MessengerInstance.Register<LaunchEditPlanItemPageMessage>(this, OnLaunchEditPlanItemPage);
        }


        public ICollection<GroupedPlanItemViewModel> PlanItems
        {
            get;
            private set;
        }


        public void Initialize(Event currentEvent)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            this.currentEvent = currentEvent;
            tripPlan = tripPlanService.GetTripPlan(currentEvent);
      
            if (tripPlan != null)
            {
                var planItemsViewModel = new List<IPlanItemViewModel>();
                AddPlanItems(planItemsViewModel, tripPlan.PlanItems);
                AddPlanItems(planItemsViewModel, tripPlan.FlightPlanItems);
                AddPlanItems(planItemsViewModel, tripPlan.RentPlanItems);

                PlanItems = new List<GroupedPlanItemViewModel>(GroupPlanItems(planItemsViewModel));
            }
            else
            {
                PlanItems.Clear();
            }
            RaisePropertyChanged("PlanItems");
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("Trip plan loaded = " + watch.ElapsedMilliseconds);
        }


        public override void Cleanup()
        {
            PlanItems = null;
        }


        public void ShowPlanItemsOnMap()
        {
            var planItems = new List<BasePlanItem>();
            foreach (var planItemGroup in PlanItems)
            {
                foreach (var planItem in planItemGroup)
                {
                    planItems.Add(planItem.BasePlanItem);
                }
            }
            MessengerInstance.Send<ShowPlanItemPushpinsOnMapMessage>(new ShowPlanItemPushpinsOnMapMessage(planItems));
        }


        private void AddPlanItems<T>(ICollection<IPlanItemViewModel> planItemsViewModel, ICollection<T> planItems) where T : BasePlanItem
        {
            foreach (var planItem in planItems)
            {
                foreach (var vm in PlanItemViewModelFactory.CreatePlanItemViewModel(planItem))
                {
                    planItemsViewModel.Add(vm);
                }
            }
        }


        private IEnumerable<GroupedPlanItemViewModel> GroupPlanItems(ICollection<IPlanItemViewModel> planItems)
        {
            var groupedPlanItems =
                from planItem in planItems
                orderby planItem.Date ascending
                group planItem by planItem.Date != null ?
                                  planItem.Date.Value.ToString(AppResources.Short_Dayly_Date_Format) :
                                  BasePlanItemViewModel<BasePlanItem>.UNDATED_LABEL into photosByDay
                select new GroupedPlanItemViewModel(photosByDay);
            return groupedPlanItems;
        }


        private void OnPlanItemSaved(PlanItemSavedMessage message)
        {
            tripPlan = GetOrCreateTripPlan();
            if (tripPlan != null)
            {
                if (message.OldPlanItem == null)
                {
                    tripPlanService.AddPlanItem(tripPlan, message.PlanItem, currentEvent);
                }
                else
                {
                    tripPlanService.UpdatePlanItem(tripPlan, message.OldPlanItem, message.PlanItem, currentEvent);
                }
                Initialize(currentEvent);
            }
        }


        private void OnPlanItemDeleted(PlanItemDeletedMessage message)
        {
            tripPlan = GetOrCreateTripPlan();
            if (tripPlan != null)
            {
                tripPlanService.DeletePlanItem(tripPlan, message.PlanItem, currentEvent);
                if (PlanItems != null)
                {
                    Initialize(currentEvent);
                }
            }
        }


        private void OnLaunchEditPlanItemPage(LaunchEditPlanItemPageMessage message)
        {
            MessengerInstance.Send<EditPlanItemMessage>(new EditPlanItemMessage(message.PlanItem));
            navigationService.Navigate(EDIT_PLAN_ITEM_PATH);
        }


        private TripPlan GetOrCreateTripPlan()
        {
            if (tripPlan == null)
            {
                tripPlan = new TripPlan();
                tripPlanService.AddTripPlan(tripPlan, currentEvent);
                tripPlan = tripPlanService.GetTripPlan(currentEvent);
            }
            return tripPlan;
        }

    }
}
