using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.Auth.ViewModel;
using Ninject;
using UTraveller.Common.Message;
using UTraveller.Common.Control;

namespace UTraveller.Auth
{
    public partial class UserSignInPage : BasePhoneApplicationPage
    {
        public UserSignInPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                App.Messenger.Register<UserAuthenticatedMessage>(typeof(UserSignInPage), OnUserAuthenticated);
                var viewModel = App.IocContainer.Get<UserSignInViewModel>();
                DataContext = viewModel;
                viewModel.Initialize();
            }
        }


        private void OnUserAuthenticated(UserAuthenticatedMessage message)
        {
            if (message.Object)
            {
                if (NavigationService.BackStack.Count() == 2)
                {
                    NavigationService.RemoveBackEntry();
                }
                if (NavigationService.BackStack.Count() > 1)
                {
                    NavigationService.RemoveBackEntry();
                    NavigationService.RemoveBackEntry();
                }
                NavigationService.GoBack();
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode == NavigationMode.Back)
            {
                App.Messenger.Unregister<UserAuthenticatedMessage>(typeof(UserSignInPage));
                ((UserSignInViewModel)DataContext).Cleanup();
                DataContext = null;
            }
        }


        private void CloseButtonClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }


        private void PhoneTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (PhoneTextBox)sender;
            if (textBox.Text.Length >= 1)
            {
                textBox.PlaceholderText = string.Empty;
            }
        }


        private void PhonePasswordBoxTextChanged(object sender, System.Windows.Input.KeyEventArgs e)
        {
            passwordPlaceholder.PlaceholderText = string.Empty;
        }


        private void RequestResetPasswordButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (requestResetPasswordPanel.Visibility == Visibility.Collapsed)
            {
                requestResetPasswordPanel.Visibility = Visibility.Visible;
            }
            else
            {
                requestResetPasswordPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}