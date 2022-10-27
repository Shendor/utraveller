using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using GalaSoft.MvvmLight;
using Ninject;
using UTraveller.ImageChooser.ViewModel;
using UTraveller.ImageChooser.Messages;

namespace UTraveller.ImageChooser.Control
{
    public partial class MessageChooser : PhoneApplicationPage
    {
        public MessageChooser()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);

            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                var viewModel = App.IocContainer.Get<MessagesChooserViewModel>();
                viewModel.Initialize();
                DataContext = viewModel;
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                var viewModel = DataContext as ViewModelBase;
                if (viewModel != null)
                {
                    viewModel.Cleanup();
                }
            }
        }


        private void CloseButtonClick(object sender, EventArgs e)
        {
            ClosePage();
        }


        private void OnAddedPhotos(MessagesChosenMessage message)
        {
            ClosePage();
        }


        private void ClosePage()
        {
            NavigationService.GoBack();
        }
    }
}