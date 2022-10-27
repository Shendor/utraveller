using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using UTraveller.Common.ViewModel;
using Ninject;
using UTraveller.Service.Api;

namespace UTraveller.Common.Control
{
    public class BasePhoneApplicationPage : PhoneApplicationPage, IActivateablePage
    {
        public BasePhoneApplicationPage()
        {
            OrientationChanged += BasePhoneApplicationPage_OrientationChanged;
        }

        private void BasePhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            var notificationService = App.IocContainer.Get<NotificationService>();
            notificationService.ChangeOrientation(e.Orientation);
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            HideNotification();
            if (HasRunBackgroundTasks())
            {
                e.Cancel = true;
            }
            else
            {
                base.OnBackKeyPress(e);
            }
        }


        protected void Close()
        {
            HideNotification();
            if (!HasRunBackgroundTasks())
            {
                NavigationService.GoBack();
            }
        }


        private bool HasRunBackgroundTasks()
        {
            //var progressService = App.IocContainer.Get<ITaskProgressService>();
            var cancelableProgressService = App.IocContainer.Get<ICancelableTaskProgressService>();

            return cancelableProgressService.IsInProgress;// || progressService.IsInProgress;
        }


        private void HideNotification()
        {
            var notificationService = App.IocContainer.Get<NotificationService>();
            if (notificationService.IsVisible)
            {
                notificationService.Hide();
            }

            var routePushpinDescriptionService = App.IocContainer.Get<PushpinDescriptionService>();
            if (routePushpinDescriptionService.IsVisible)
            {
                routePushpinDescriptionService.Hide();
            }
        }


        public virtual void Activate()
        {
            if (HasRunBackgroundTasks())
            {
                var cancelableProgressService = App.IocContainer.Get<ICancelableTaskProgressService>();

                cancelableProgressService.CancelTask();
            }
        }


        public virtual void Deactivate()
        {
        }
    }
}
