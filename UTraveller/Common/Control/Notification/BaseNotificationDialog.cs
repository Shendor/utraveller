using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UTraveller.Common.Control
{
    public abstract class BaseNotificationDialog : UserControl
    {
        private Popup popup;
        private NotificationService notificationService;
        private CompositeTransform transform;

        protected virtual void InitializePopup(CompositeTransform transform)
        {
            this.transform = transform;
            popup = new Popup();
            this.popup.Child = this;
        }


        private void ChangeOrientation(PageOrientation pageOrientation)
        {
            if (pageOrientation == PageOrientation.LandscapeLeft)
            {
                Width = Application.Current.Host.Content.ActualHeight;
                Height = Application.Current.Host.Content.ActualWidth;

                transform.CenterX = 0;
                transform.CenterY = 0;

                transform.Rotation = 90;

                transform.TranslateY = 0;
                transform.TranslateX = Height;
            }
            else if (pageOrientation == PageOrientation.LandscapeRight)
            {
                Width = Application.Current.Host.Content.ActualHeight;
                Height = Application.Current.Host.Content.ActualWidth;

                transform.CenterX = 0;
                transform.CenterY = 0;

                transform.Rotation = -90;

                transform.TranslateY = Width;
                transform.TranslateX = 0;
            }
            else
            {
                Width = Application.Current.Host.Content.ActualWidth;
                Height = Application.Current.Host.Content.ActualHeight;

                transform.Rotation = 0;
                transform.TranslateY = 0;
                transform.TranslateX = 0;
            }
        }


        protected void OnNotificationChanged(NotificationChangedMessage message)
        {
            if (!message.Object)
            {
                DataContext = null;
            }
            else
            {
                // do it in dispatcher bacuse colors are not refreshed after sign in
                Dispatcher.BeginInvoke(() =>
                {
                    DataContext = null;
                    DataContext = notificationService = GetNotificationService();
                });
            }
            ChangeOrientation(message.Orientation);
            popup.IsOpen = message.Object;
        }


        protected abstract NotificationService GetNotificationService();


        protected abstract object GetMessageToken();


        protected void Close()
        {
            DataContext = null;
            if (notificationService != null)
            {
                notificationService.Hide();
            }
        }
    }
}
