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
    public partial class UserRegistrationPage : BasePhoneApplicationPage
    {
        private UserRegistrationViewModel viewModel;

        public UserRegistrationPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                App.Messenger.Register<UserRegisteredMessage>(typeof(UserRegistrationPage), OnUserRegistered);
                viewModel = App.IocContainer.Get<UserRegistrationViewModel>();
                viewModel.Initialize();
                DataContext = viewModel;
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode == NavigationMode.Back)
            {
                App.Messenger.Unregister<UserRegisteredMessage>(typeof(UserRegistrationPage));
                viewModel.Cleanup();
                DataContext = null;
            }
        }


        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            GoBackOrCloseApp();
        }


        private void CloseButtonClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            GoBackOrCloseApp();
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


        private void PhoneRepeatPasswordBoxTextChanged(object sender, System.Windows.Input.KeyEventArgs e)
        {
            repeatPasswordPlaceholder.PlaceholderText = string.Empty;
        }


        private void OnUserRegistered(UserRegisteredMessage message)
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


        private void GoBackOrCloseApp()
        {
            if (!viewModel.IsUserAuthenticated)
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