using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.Sync.ViewModel;
using Ninject;
using UTraveller.Common.Control;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Sync
{
    public partial class SyncPage : BasePhoneApplicationPage
    {
        private SyncViewModel viewModel;

        public SyncPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                viewModel = App.IocContainer.Get<SyncViewModel>();
                DataContext = viewModel;
                viewModel.Initialize();
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode == NavigationMode.Back && viewModel != null)
            {
                viewModel.Cleanup();
                DataContext = null;
                viewModel = null;
            }
        }


        private void CloseButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close();
        }


        private void TimeLineItemTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var tag = ((FrameworkElement)sender).Tag;
            if (tag is BaseModel)
            {
                viewModel.ShowDescription(tag as BaseModel);
            }
        }
    }
}