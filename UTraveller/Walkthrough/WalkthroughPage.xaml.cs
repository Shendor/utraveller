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
using UTraveller.Walkthrough.ViewModel;

namespace UTraveller.Walkthrough
{
    public partial class WalkthroughPage : PhoneApplicationPage
    {
        private bool isFirstRun;

        public WalkthroughPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                var parameter = NavigationContext.QueryString[UTraveller.Service.Implementation.NavigationService.PARAMETER_NAME];
                var viewModel = App.IocContainer.Get<WalkthroughViewModel>();
                if (Boolean.Parse(parameter))
                {
                    lastItem.Visibility = Visibility.Visible;
                    isFirstRun = true;
                }
                else
                {
                    flipView.Items.Remove(lastItem);
                    isFirstRun = false;
                }

                DataContext = viewModel;
            } 
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ((WalkthroughViewModel)DataContext).Cleanup();
            }
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (isFirstRun)
            {
                App.ReleaseDatabase();
                App.Current.Terminate();
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}