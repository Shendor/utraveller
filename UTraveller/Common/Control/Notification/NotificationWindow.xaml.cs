using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;
using UTraveller.Common.ViewModel;
using Ninject;

namespace UTraveller.Common.Control
{
    public partial class NotificationWindow : BaseNotificationDialog
    {
        public NotificationWindow()
        {
            InitializeComponent();
            InitializePopup(transform);
            App.Messenger.Register<NotificationChangedMessage>(typeof(NotificationWindow), GetMessageToken(), OnNotificationChanged);
        }


        protected override NotificationService GetNotificationService()
        {
            return App.IocContainer.Get<NotificationService>();
        }


        private void ControlTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close();
        }


        protected override object GetMessageToken()
        {
            return NotificationService.MESSAGE_TOKEN;
        }
    }
}
