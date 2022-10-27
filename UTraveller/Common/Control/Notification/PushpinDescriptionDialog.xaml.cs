using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;

namespace UTraveller.Common.Control
{
    public partial class PushpinDescriptionDialog : BaseNotificationDialog
    {
        public PushpinDescriptionDialog()
        {
            InitializeComponent();
            InitializePopup(transform);
            App.Messenger.Register<NotificationChangedMessage>(typeof(PushpinDescriptionDialog), GetMessageToken(), OnNotificationChanged);
        }

        private void DriveToButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ((PushpinDescriptionService)GetNotificationService()).DriveTo();
        }


        private void WalkToButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ((PushpinDescriptionService)GetNotificationService()).WalkTo();
        }


        protected override NotificationService GetNotificationService()
        {
            return App.IocContainer.Get<PushpinDescriptionService>();
        }


        private void ControlTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close();
        }


        protected override object GetMessageToken()
        {
            return PushpinDescriptionService.MESSAGE_TOKEN;
        }
    }
}
