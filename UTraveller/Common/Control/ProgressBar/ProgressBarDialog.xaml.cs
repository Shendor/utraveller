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
using Ninject;
using UTraveller.Service.Api;

namespace UTraveller.Common.Control.ProgressBar
{
    public partial class ProgressBarDialog : UserControl
    {
        private Popup popup;

        public ProgressBarDialog()
        {
            InitializeComponent();
            popup = new Popup();
            this.popup.Child = this;
            App.Messenger.Register<ProgressBarChangedMessage>(typeof(ProgressBarDialog), 
               TaskProgressService.TOKEN, OnProgressBarChanged);
        }


        private void OnProgressBarChanged(ProgressBarChangedMessage message)
        {
            DataContext = null;
            DataContext = App.IocContainer.Get<ICancelableTaskProgressService>();

            Width = Application.Current.Host.Content.ActualWidth;
            Height = Application.Current.Host.Content.ActualHeight;
            popup.IsOpen = message.IsOpened;

            progress.Maximum = message.MaxValue;
            progress.Value = message.Value;
        }
    }
}
