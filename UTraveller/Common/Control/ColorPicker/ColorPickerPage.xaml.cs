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
using UTraveller.Common.Message;

namespace UTraveller.Common.Control
{
    public partial class ColorPickerPage : PhoneApplicationPage
    {
        public ColorPickerPage()
        {
            InitializeComponent();
        }
        
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                var viewModel = DataContext as ColorPickerViewModel;
                if (viewModel != null)
                {
                    App.Messenger.Unregister<ColorChosenMessage>(typeof(ColorPickerPage));
                    DataContext = null;
                }
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                var viewModel = App.IocContainer.Get<ColorPickerViewModel>();
                DataContext = viewModel;
                App.Messenger.Register<ColorChosenMessage>(typeof(ColorPickerPage), viewModel.Token, OnImageChosen);
            }
        }


        private void OnImageChosen(ColorChosenMessage message)
        {
            NavigationService.GoBack();
        }


        private void CloseButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}