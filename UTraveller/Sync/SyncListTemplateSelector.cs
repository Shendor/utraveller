using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UTraveller.Common.Control;
using UTraveller.Sync.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Sync
{
    public class SyncListTemplateSelector : DataTemplateSelector
    {
        public DataTemplate EventDataTemplate
        {
            get;
            set;
        }


        public DataTemplate PhotoDataTemplate
        {
            get;
            set;
        }


        public DataTemplate MessageDataTemplate
        {
            get;
            set;
        }


        public DataTemplate MoneySpendingDataTemplate
        {
            get;
            set;
        }


        public DataTemplate RouteDataTemplate
        {
            get;
            set;
        }

        public DataTemplate TripPlanDataTemplate
        {
            get;
            set;
        }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var timeLineItem = item as SyncEntityViewModel;
            if (timeLineItem != null)
            {
                if (timeLineItem.Entity is Event)
                {
                    return EventDataTemplate;
                }
                else if (timeLineItem.Entity is Photo)
                {
                    return PhotoDataTemplate;
                }
                else if (timeLineItem.Entity is Message)
                {
                    return MessageDataTemplate;
                }
                else if (timeLineItem.Entity is MoneySpending)
                {
                    return MoneySpendingDataTemplate;
                }
                else if (timeLineItem.Entity is Route)
                {
                    return RouteDataTemplate;
                }
                else if (timeLineItem.Entity is TripPlan)
                {
                    return TripPlanDataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
