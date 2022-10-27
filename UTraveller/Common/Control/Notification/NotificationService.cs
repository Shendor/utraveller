using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.ViewModel;

namespace UTraveller.Common.Control
{
    public class NotificationService : BaseViewModel
    {
        public static readonly string MESSAGE_TOKEN = typeof(NotificationService).Name;

        private PageOrientation pageOrientation;

        public NotificationService()
        {
            MessageToken = MESSAGE_TOKEN;
        }

        public virtual object MessageToken
        {
            get;
            protected set;
        }


        public string Text
        {
            get;
            private set;
        }


        public bool IsVisible
        {
            get;
            set;
        }


        public void Show(string message)
        {
            IsVisible = true;
            Text = message;
            MessengerInstance.Send<NotificationChangedMessage>(new NotificationChangedMessage(true, pageOrientation), MessageToken);
            RaisePropertyChanged("Text");
        }


        public void Hide()
        {
            IsVisible = false;
            MessengerInstance.Send<NotificationChangedMessage>(new NotificationChangedMessage(false), MessageToken);
        }


        public void ChangeOrientation(PageOrientation pageOrientation)
        {
            this.pageOrientation = pageOrientation;
            if (IsVisible)
            {
                MessengerInstance.Send<NotificationChangedMessage>(new NotificationChangedMessage(IsVisible, pageOrientation));
            }
        }
    }
}
