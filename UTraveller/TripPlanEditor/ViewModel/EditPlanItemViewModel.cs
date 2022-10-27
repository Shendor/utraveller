using Microsoft.Expression.Interactivity.Core;
using Microsoft.Phone.Maps.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Control;
using UTraveller.Common.Control.Dialog;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTraveller.EventMap.Messages;
using UTraveller.Service.Api;
using UTraveller.TripPlanEditor.Messages;
using UTraveller.TripPlanEditor.Model;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.TripPlanEditor.ViewModel
{
    public class EditPlanItemViewModel : BaseViewModel
    {
        private static string previousSelectedTypeName;
        private static readonly string EDIT_PLAN_ITEM_COORDINATE_PAGE = "/TripPlanEditor/EditPlanItemCoordinatePage.xaml";

        private PlanItemTypeModel type;
        private BasePlanItem planItemToUpdate;
        private INavigationService navigationService;
        private ICancelableTaskProgressService taskProgressService;
        private IGeoCoordinateService geoLocationService;
        private NotificationService notificationService;
        private ConfirmationService confirmationService;
        private string originAddress;
        private RoutePushpin selectedRoutePushpin;

        public EditPlanItemViewModel(INavigationService navigationService, ICancelableTaskProgressService taskProgressService,
            IGeoCoordinateService geoLocationService, ConfirmationService confirmationService, NotificationService notificationService)
        {
            this.navigationService = navigationService;
            this.taskProgressService = taskProgressService;
            this.geoLocationService = geoLocationService;
            this.confirmationService = confirmationService;
            this.notificationService = notificationService;

            SavePlanItemCommand = new ActionCommand(SavePlanItem);

            MessengerInstance.Register<EditPlanItemMessage>(this, OnEditPlanItem);
            MessengerInstance.Register<EditPlanItemCoordinateMessage>(this, OnEditPlanItemCoordinate);
            MessengerInstance.Register<ViewPlanItemCoordinateMessage>(this, OnViewPlanItemCoordinate);
            MessengerInstance.Register<ApplyPlanItemCoordinate>(this, OnApplyPlanItemCoordinate);
            MessengerInstance.Register<CreatePlanItemFromRoutePushpinMessage>(this, OnCreatePlanItemFromRoutePushpin);
        }


        public void Initialize()
        {
            PlanItemTypes = new SortedSet<PlanItemTypeModel>();
            string selectedTypeForPlanItemToUpdate = GetTypeOfPlanItemToUpdate();
            PlanItemTypeModel selectedPlanItemType = null;

            foreach (var enumValue in PlanItemTypeUtil.GetAllPlanItemTypes())
            {
                var item = AddPlanItemViewModelAndReturnSelectedType(selectedTypeForPlanItemToUpdate, enumValue);
                if (item != null)
                {
                    selectedPlanItemType = item;
                }
            }
            RaisePropertyChanged("PlanItemTypes");

            if (selectedPlanItemType == null)
            {
                selectedPlanItemType = PlanItemTypes.FirstOrDefault((item) => item.SelectedPlanItemType.Equals(PlanItemType.Other));
            }
            Type = selectedPlanItemType;
            RaisePropertyChanged("Type");
            RaisePropertyChanged("IsTypeEnabled");

            if (selectedRoutePushpin != null)
            {
                PlanItemViewModel.Location = selectedRoutePushpin.Coordinate;
                PlanItemViewModel.Description = selectedRoutePushpin.Description;

                SearchAndApplyAddress(selectedRoutePushpin.Coordinate);
            }
        }


        private PlanItemTypeModel AddPlanItemViewModelAndReturnSelectedType(string selectedTypeForPlanItemToUpdate, Enum category)
        {
            var name = PlanItemTypeUtil.GetPlanItemTypeName(category);
            var item = new PlanItemTypeModel(category, name);
            PlanItemTypes.Add(item);

            if (!string.IsNullOrEmpty(selectedTypeForPlanItemToUpdate) && selectedTypeForPlanItemToUpdate.Equals(name))
            {
                return item;
            }
            else
            {
                return null;
            }
        }


        public override void Cleanup()
        {
            selectedRoutePushpin = null;
            originAddress = null;
            planItemToUpdate = null;
            PlanItemViewModel = null;
            PlanItemTypes = null;
            Type = null;
        }


        public ICommand SavePlanItemCommand
        {
            get;
            private set;
        }


        public BaseEditPlanItemViewModel PlanItemViewModel
        {
            get;
            private set;
        }


        public PlanItemTypeModel Type
        {
            get { return type; }
            set
            {
                if (value != null && (type == null || type.EditPlanItemType != value.EditPlanItemType))
                {
                    PlanItemViewModel = CreateBaseEditPlanItemViewModel(value.EditPlanItemType);
                    MessengerInstance.Send<EditPlanItemTypeChangedMessage>(new EditPlanItemTypeChangedMessage(value.EditPlanItemType));
                }
                type = value;
                if (type != null)
                {
                    previousSelectedTypeName = type.Name;
                }
            }
        }


        public ICollection<PlanItemTypeModel> PlanItemTypes
        {
            get;
            private set;
        }


        public bool IsTypeEnabled
        {
            get { return planItemToUpdate == null; }
        }


        private async void SavePlanItem()
        {
            if (PlanItemViewModel != null)
            {
                if ((!string.IsNullOrEmpty(PlanItemViewModel.Address) && PlanItemViewModel.Location == null) ||
                    (!string.IsNullOrEmpty(PlanItemViewModel.Address) && !string.IsNullOrEmpty(originAddress) && !PlanItemViewModel.Address.Equals(originAddress)
                    && PlanItemViewModel.Location != null &&
                    await confirmationService.Show("Do you want automatically assign Coordinate on the Map for this Address?")))
                {
                    try
                    {
                        taskProgressService.RunIndeterminateProgress("Searching for the Coordinate...");
                        geoLocationService.FindCoordinateForAddress(PlanItemViewModel.Address, (coordinate) =>
                        {
                            taskProgressService.FinishProgress();
                            if (coordinate != null)
                            {
                                PlanItemViewModel.Location = coordinate;
                            }
                            PlanItemViewModel.SavePlanItem(Type);
                        });
                    }
                    catch
                    {
                        taskProgressService.FinishProgress();
                    }
                }
                else
                {
                    PlanItemViewModel.SavePlanItem(Type);
                }
            }
        }

        #region Message handlers

        private void OnCreatePlanItemFromRoutePushpin(CreatePlanItemFromRoutePushpinMessage message)
        {
            selectedRoutePushpin = message.RoutePushpin;
        }


        private void OnEditPlanItem(EditPlanItemMessage message)
        {
            planItemToUpdate = message.PlanItem;
            originAddress = planItemToUpdate.Address;
        }


        private void OnEditPlanItemCoordinate(EditPlanItemCoordinateMessage message)
        {
            if (PlanItemViewModel != null && PlanItemViewModel.Location == null)
            {
                notificationService.Show("Location of the item is not recognized and reset to default. Please, assign a new one.");
            }
            navigationService.Navigate(EDIT_PLAN_ITEM_COORDINATE_PAGE);
        }


        private void OnViewPlanItemCoordinate(ViewPlanItemCoordinateMessage message)
        {
            if (message.Coordinate == null)
            {
                notificationService.Show("This item doesn't have coordinate on the map :(");
            }
            else
            {
                navigationService.Navigate(EDIT_PLAN_ITEM_COORDINATE_PAGE);
            }
        }


        private void OnApplyPlanItemCoordinate(ApplyPlanItemCoordinate message)
        {
            if (PlanItemViewModel != null)
            {
                SearchAndApplyAddress(message.Coordinate);
            }
        }

        #endregion

        #region Private methods

        private void SearchAndApplyAddress(GeoCoordinate coordinate)
        {
            try
            {
                taskProgressService.RunIndeterminateProgress("Searching for the Address...");
                PlanItemViewModel.Location = coordinate;

                geoLocationService.FindAddressForCoordinate(PlanItemViewModel.Location, (address) =>
                {
                    taskProgressService.FinishProgress();
                    PlanItemViewModel.Address = address.ToString();
                });
            }
            catch
            {
                taskProgressService.FinishProgress();
            }
        }


        private BaseEditPlanItemViewModel CreateBaseEditPlanItemViewModel(EditPlanItemType editPlanItemType)
        {
            if (editPlanItemType == EditPlanItemType.Other)
            {
                return new EditOtherPlanItemViewModel(planItemToUpdate);
            }
            else if (editPlanItemType == EditPlanItemType.Rent && (planItemToUpdate == null || planItemToUpdate is RentPlanItem))
            {
                return new EditRentPlanItemViewModel((RentPlanItem)planItemToUpdate);
            }
            else if (planItemToUpdate == null || planItemToUpdate is TransportPlanItem)
            {
                return new EditTransportPlanItemViewModel((TransportPlanItem)planItemToUpdate);
            }
            else
            {
                return null;
            }
        }


        private string GetTypeOfPlanItemToUpdate()
        {
            string selectedTypeForPlanItemToUpdate = previousSelectedTypeName;
            if (planItemToUpdate != null)
            {
                if (planItemToUpdate is PlanItem)
                {
                    selectedTypeForPlanItemToUpdate = PlanItemTypeUtil.GetPlanItemTypeName(((PlanItem)planItemToUpdate).Type);
                }
                else if (planItemToUpdate is RentPlanItem)
                {
                    selectedTypeForPlanItemToUpdate = PlanItemTypeUtil.GetPlanItemTypeName(((RentPlanItem)planItemToUpdate).Type);
                }
                else
                {
                    selectedTypeForPlanItemToUpdate = PlanItemTypeUtil.GetPlanItemTypeName(((TransportPlanItem)planItemToUpdate).Type);
                }
            }
            return selectedTypeForPlanItemToUpdate;
        }
        #endregion
    }
}
