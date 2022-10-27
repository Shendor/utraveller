using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UTraveller.Common.Control;
using UTraveller.Common.ViewModel;
using UTraveller.EventDetails.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.Control
{
    public class EventDetailsTimeLineItemTemplateSelector : DataTemplateSelector
    {
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

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var timeLineItem = item as ITimeLineItem<IDateItem>;
            if (timeLineItem != null)
            {
                if (timeLineItem is PhotoViewModel)
                {
                    return PhotoDataTemplate;
                }
                else 
                {
                    return MessageDataTemplate;
                }
            }

            return base.SelectTemplate(item, container);
        }
    }
}
