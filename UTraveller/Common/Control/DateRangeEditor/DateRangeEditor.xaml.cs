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

namespace UTraveller.Common.Control.DateRangeEditor
{
    public partial class DateRangeEditor : PhoneApplicationPage
    {
        public DateRangeEditor()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                var viewModel = DataContext as DateRangeEditorViewModel;
                if (viewModel != null)
                {
                    App.Messenger.Unregister<DateRangeChosenMessage>(typeof(DateRangeEditor));
                    DataContext = null;
                }
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                var viewModel = App.IocContainer.Get<DateRangeEditorViewModel>();
                DataContext = viewModel;
                App.Messenger.Register<DateRangeChosenMessage>(typeof(DateRangeEditor), viewModel.Token, OnDateRangeChosen);
            }
        }


        private void OnDateRangeChosen(DateRangeChosenMessage message)
        {
            NavigationService.GoBack();
        }


        private void CloseButtonClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}