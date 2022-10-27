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
using System.ComponentModel;

namespace UTraveller.Common.Control.Dialog
{
    public partial class ConfirmationDialog : UserControl
    {
        private Popup popup;

        public ConfirmationDialog()
        {
            InitializeComponent();
            popup = new Popup();
            this.popup.Child = this;
            App.Messenger.Register<ShowConfirmDialogMessage>(typeof(ConfirmationDialog), OnShowConfirmDialog);
        }


        private void OnShowConfirmDialog(ShowConfirmDialogMessage message)
        {
            DataContext = App.IocContainer.Get<ConfirmationService>();
            Width = Application.Current.Host.Content.ActualWidth;
            Height = Application.Current.Host.Content.ActualHeight;
            popup.IsOpen = message.IsVisible;
        }


        private void CancelButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DataContext = null;
            popup.IsOpen = false;
        }
    }
}
