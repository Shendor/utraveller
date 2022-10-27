using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.MessagePost.ViewModel;
using UTraveller.MessagePost.Messages;
using Ninject;

namespace UTraveller.MessagePost
{
    public partial class MessagePostPage : PhoneApplicationPage
    {
        private MessagePostViewModel viewModel;

        public MessagePostPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                var parameter = NavigationContext.QueryString[UTraveller.Service.Implementation.NavigationService.PARAMETER_NAME];

                viewModel = App.IocContainer.Get<MessagePostViewModel>();
                viewModel.Token = parameter;
                DataContext = viewModel;
                App.Messenger.Register<MessagePostedMessage>(typeof(MessagePostPage), viewModel.Token, OnMessagePosted);
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                App.Messenger.Unregister<MessagePostedMessage>(typeof(MessagePostPage), viewModel.Token);
                DataContext = null;
            }
        }


        private void OnMessagePosted(MessagePostedMessage message)
        {
            NavigationService.GoBack();
        }

    }
}