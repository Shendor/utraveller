using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.EventList.ViewModel;
using Ninject;

namespace UTraveller.EventList
{
    public partial class EventPage : PhoneApplicationPage
    {
        public EventPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                var viewModel = DataContext as EventViewModel;
                if (viewModel != null)
                {
                    viewModel.Cleanup();
                }
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                var viewModel = App.IocContainer.Get<EventViewModel>();
                DataContext = viewModel;
               // viewModel.Initialize();
            }
        }
    }
}