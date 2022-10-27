using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTraveller.Resources;
using UTraveller.TripPlanEditor.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public abstract class BasePlanItemViewModel<T> : BaseViewModel, IPlanItemViewModel where T : BasePlanItem
    {
        public static readonly string UNDATED_LABEL = "Undated";

        protected T basePlanItem;

        public BasePlanItemViewModel(T basePlanItem)
        {
            this.basePlanItem = basePlanItem;
            DeletePlanItemCommand = new ActionCommand(DeletePlanItem);
            ViewLocationCommand = new ActionCommand(ViewLocation);
            EditPlanItemCommand = new ActionCommand(EditPlanItem);
        }

        public ICommand DeletePlanItemCommand
        {
            get;
            private set;
        }


        public ICommand EditPlanItemCommand
        {
            get;
            private set;
        }


        public ICommand ViewLocationCommand
        {
            get;
            private set;
        }


        public BasePlanItem BasePlanItem
        {
            get { return basePlanItem; }
        }


        public virtual DateTime? Date
        {
            get { return basePlanItem.Date; }
        }


        public virtual string ShortFormattedDate
        {
            get
            {
                return Date != null ?
                    Date.Value.ToString(AppResources.Short_No_Week_Name_Date_Format, App.CurrentCulture) : UNDATED_LABEL;
            }
        }

        public virtual string Description
        {
            get { return basePlanItem.Description; }
        }


        public abstract string Caption
        {
            get;
        }


        public abstract Uri Icon
        {
            get;
        }


        private void DeletePlanItem()
        {
            MessengerInstance.Send<PlanItemDeletedMessage>(new PlanItemDeletedMessage(basePlanItem));
        }


        public void EditPlanItem()
        {
            MessengerInstance.Send<LaunchEditPlanItemPageMessage>(new LaunchEditPlanItemPageMessage(basePlanItem));
        }


        private void ViewLocation()
        {
            if (basePlanItem != null)
            {
                MessengerInstance.Send<ViewPlanItemCoordinateMessage>(new ViewPlanItemCoordinateMessage(basePlanItem.Address, basePlanItem.Coordinate));
            }
        }


        public int CompareTo(IPlanItemViewModel other)
        {
            if (Date != null && other.Date != null)
            {
                return basePlanItem.Date.Value.CompareTo(other.Date.Value);
            }
            else
            {
                return 0;
            }
        }
    }
}
