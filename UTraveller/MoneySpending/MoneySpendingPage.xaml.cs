using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.MoneySpendings.ViewModel;
using Ninject;
using UTraveller.Resources;
using UTraveller.MoneySpendings.Message;
using UTraveller.Common.Control;

namespace UTraveller.MoneySpendings
{
    public partial class MoneySpendingPage : BasePhoneApplicationPage
    {
        public MoneySpendingPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                DataContext = App.IocContainer.Get<MoneySpendingViewModel>();
                ((MoneySpendingViewModel)DataContext).Initialize();
                App.Messenger.Register<MoneySpendingAddedMessage>(typeof(MoneySpendingPage), OnMoneySpendingAdded);
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ((MoneySpendingViewModel)DataContext).Cleanup();
                App.Messenger.Unregister<MoneySpendingAddedMessage>(typeof(MoneySpendingPage));
            }
        }

        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }


        private void AmountTextBoxTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }


        private void OnMoneySpendingAdded(MoneySpendingAddedMessage message)
        {
            Close();
        }

    }
}