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
using UTraveller.Service.Api;
using Ninject;

namespace UTraveller.Common.Control.ProgressBar
{
    public partial class BackgroundProgressBar : System.Windows.Controls.ProgressBar
    {
        private Popup popup;

        public BackgroundProgressBar()
        {
            InitializeComponent();
            popup = new Popup();
            this.popup.Child = this;
            popup.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            App.Messenger.Register<ProgressBarChangedMessage>(typeof(ProgressBarDialog),
                BackgroundTaskProgressService.TOKEN, OnProgressBarChanged);
        }


        private void OnProgressBarChanged(ProgressBarChangedMessage message)
        {
            DataContext = null;
            DataContext = App.IocContainer.Get<ITaskProgressService>("backgroundTaskProgressService");

            Width = Application.Current.Host.Content.ActualWidth;
            Height = 15;
            popup.IsOpen = message.IsOpened;
        }
    }
}
