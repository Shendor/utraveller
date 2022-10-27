using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UTraveller.Common.Message;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTraveller.EventMap.Messages;
using UTraveller.Resources;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel.Map
{
    public class PlanItemLegendViewModel : BaseViewModel
    {
        private bool isVisible;

        public PlanItemLegendViewModel(SolidColorBrush color, string day, bool isVisible = true)
        {
            this.Color = color;
            this.Day = day;
            this.isVisible = isVisible;
        }

        public ICollection<PlanItemPushpinViewModel> PlanItemPushpins
        {
            get;
            set;
        }


        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                MessengerInstance.Send<FilterPlanItemPushpinMessage>(new FilterPlanItemPushpinMessage(this));
            }
        }


        public string Day
        {
            get;
            private set;
        }


        public Brush Color
        {
            get;
            private set;
        }
    }


    public class PlanItemsPushpinsViewModel : BaseViewModel
    {
        private int dayNumber;

        public PlanItemsPushpinsViewModel()
        {
            PlanItemsLegend = new ObservableCollection<PlanItemLegendViewModel>();
            PlanItemPushpins = new ObservableCollection<PlanItemPushpinViewModel>();

            MessengerInstance.Register<FilterPlanItemPushpinMessage>(this, OnFilterPlanItemPushpin);
        }


        public ICollection<PlanItemPushpinViewModel> PlanItemPushpins
        {
            get;
            private set;
        }


        public ICollection<PlanItemLegendViewModel> PlanItemsLegend
        {
            get;
            private set;
        }


        public void AddPushpin(PlanItemPushpinViewModel planItemPushpin)
        {
            if (planItemPushpin.BasePlanItem.Coordinate != null)
            {
                PlanItemPushpins.Add(planItemPushpin);
            }
        }


        public void DeletePushpin(PlanItemPushpinViewModel planItemPushpin)
        {
            PlanItemPushpins.Remove(planItemPushpin);
        }


        public void AddPlanItemLegend(PlanItemLegendViewModel planItemLegend)
        {
            PlanItemsLegend.Add(planItemLegend);
        }


        public void ClearPushpins()
        {
            dayNumber = 0;
            PlanItemPushpins.Clear();
            PlanItemsLegend.Clear();
        }


        public void SavePlanItem(BasePlanItem oldPlanItem, BasePlanItem planItem)
        {
            PlanItemPushpinViewModel planItemPushpinToDelete =
                PlanItemPushpins.FirstOrDefault((p) => p.BasePlanItem.Equals(oldPlanItem));
            if (planItemPushpinToDelete != null)
            {
                DeletePushpin(planItemPushpinToDelete);
            }
            var savedPlanItemViewModel = new PlanItemPushpinViewModel(planItem);
            AssignColorForPlanItemPushpin(savedPlanItemViewModel, PlanItemPushpins);
            AddPushpin(savedPlanItemViewModel);
        }


        public void ShowPlanItems(IList<BasePlanItem> planItems, bool isPlanItemPushpinsVisible)
        {
            foreach (var basePlanItem in planItems)
            {
                if (basePlanItem.Coordinate != null)
                {
                    var item = new PlanItemPushpinViewModel(basePlanItem);
                    item.Visibility = isPlanItemPushpinsVisible && !basePlanItem.IsVisited ? Visibility.Visible : Visibility.Collapsed;
                    AddPushpin(item);
                }
            }
            AddAndAssignColorsForPlanItemPushpins(PlanItemPushpins);
        }


        private void OnFilterPlanItemPushpin(FilterPlanItemPushpinMessage message)
        {
            if (message.PlanItemLegend.Day.Equals("Visited")) // TODO: refactor
            {
                foreach (var planItem in PlanItemPushpins)
                {
                    if (planItem.BasePlanItem.IsVisited)
                    {
                        planItem.Visibility = message.PlanItemLegend.IsVisible ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
            else
            {
                foreach (var planItem in PlanItemPushpins)
                {
                    if (!planItem.BasePlanItem.IsVisited && planItem.Day != null && planItem.Day.Equals(message.PlanItemLegend.Day))
                    {
                        planItem.Visibility = message.PlanItemLegend.IsVisible ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
        }

        private void AssignColorForPlanItemPushpin(PlanItemPushpinViewModel planItem,
            ICollection<PlanItemPushpinViewModel> planItems)
        {
            if (planItem.BasePlanItem.Date != null)
            {
                bool isDayFound = false;
                planItem.Day = planItem.BasePlanItem.Date.Value.ToString(AppResources.Short_Dayly_Date_Format);
                foreach (var planItemViewModel in planItems)
                {
                    if (planItemViewModel.Day.Equals(planItem.Day))
                    {
                        isDayFound = true;
                        planItem.Color = planItemViewModel.Color;
                        break;
                    }
                }
                if (!isDayFound)
                {
                    planItem.Color = new SolidColorBrush(ColorGenerator.GetRandomColor(dayNumber++));
                }
            }
            else
            {
                planItem.Day = AppResources.PlanItem_Undated;
            }  
        }


        private void AddAndAssignColorsForPlanItemPushpins(ICollection<PlanItemPushpinViewModel> planItems)
        {
            var groupedPlanItems =
                from planItem in planItems
                orderby planItem.BasePlanItem.Date ascending
                group planItem by planItem.BasePlanItem.Date != null ?
                                  planItem.BasePlanItem.Date.Value.ToString(AppResources.Short_Dayly_Date_Format) : AppResources.PlanItem_Undated into photosByDay
                select new
                {
                    ViewModels = photosByDay,
                    Day = photosByDay.FirstOrDefault() != null && photosByDay.FirstOrDefault().BasePlanItem.Date != null ?
                          photosByDay.FirstOrDefault().BasePlanItem.Date.Value.ToString(AppResources.Short_Dayly_Date_Format) : AppResources.PlanItem_Undated
                };

            AddPlanItemLegend(new PlanItemLegendViewModel(null, "Visited", false)); // TODO: refactor
            dayNumber = 0;
            foreach (var planItem in groupedPlanItems)
            {
                var color = planItem.Day.Equals(AppResources.PlanItem_Undated) ? null : new SolidColorBrush(ColorGenerator.GetRandomColor(dayNumber));
                foreach (var viewModel in planItem.ViewModels)
                {
                    viewModel.Color = color;
                    viewModel.Day = planItem.Day;
                }
                AddPlanItemLegend(new PlanItemLegendViewModel(color, planItem.Day));
                dayNumber++;
            }
        }

    }
}
