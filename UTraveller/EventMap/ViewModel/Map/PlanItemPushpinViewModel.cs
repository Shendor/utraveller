using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTraveller.TripPlanEditor.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel
{
    public class PlanItemPushpinViewModel : BasePushpinViewModel
    {
        private Brush color;

        public PlanItemPushpinViewModel(BasePlanItem basePlanItem)
        {
            BasePlanItem = basePlanItem;
            EditPlanItemCommand = new ActionCommand(EditPlanItem);
            DeletePlanItemCommand = new ActionCommand(DeletePlanItem);
        }


        public ICommand EditPlanItemCommand
        {
            get;
            private set;
        }


        public ICommand DeletePlanItemCommand
        {
            get;
            private set;
        }


        public int DayNumber
        {
            get;
            set;
        }


        public string Day
        {
            get;
            set;
        }


        public BasePlanItem BasePlanItem
        {
            get;
            set;
        }


        public bool IsDateDefined
        {
            get { return BasePlanItem.Date != null; }
        }


        public Brush Color
        {
            get { return color == null ? BackgroundColor : color; }
            set { color = value; }
        }


        public Brush StrokeColor
        {
            get { return color == null ? MainColor : BackgroundColor; }
        }


        public Uri Icon
        {
            get
            {
                if (BasePlanItem is PlanItem)
                {
                    return PlanItemTypeUtil.GetIcon(((PlanItem)BasePlanItem).Type);
                }
                else if (BasePlanItem is RentPlanItem)
                {
                    return PlanItemTypeUtil.GetIcon(((RentPlanItem)BasePlanItem).Type);
                }
                else
                {
                    return PlanItemTypeUtil.GetIcon(((TransportPlanItem)BasePlanItem).Type);
                }
            }
        }


        private void EditPlanItem()
        {
            MessengerInstance.Send<LaunchEditPlanItemPageMessage>(new LaunchEditPlanItemPageMessage(BasePlanItem));
        }


        private void DeletePlanItem()
        {
            MessengerInstance.Send<PlanItemDeletedMessage>(new PlanItemDeletedMessage(BasePlanItem));
        }
    }
}
