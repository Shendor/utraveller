using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Controls.Primitives;
using Ninject;
using UTraveller.MoneySpendings.ViewModel;

namespace UTraveller.MoneySpendings.Control
{
    public partial class MoneySpendingDetailsControl : UserControl
    {
        private Popup popup;
        private MoneySpendingDetailsViewModel viewModel;

        public MoneySpendingDetailsControl()
        {
            InitializeComponent();
            popup = new Popup();
            this.popup.Child = this;
        }


        public void Show(MoneySpendingItemViewModel moneySpendingItem)
        {
            if (moneySpendingItem != null)
            {
                if (viewModel == null)
                {
                    viewModel = new MoneySpendingDetailsViewModel();
                }
                viewModel.MoneySpendingItem = moneySpendingItem;
                Width = Application.Current.Host.Content.ActualWidth;
                Height = Application.Current.Host.Content.ActualHeight;
                DataContext = viewModel;
                popup.IsOpen = true;
            }
        }


        private void ControlTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            popup.IsOpen = false;
            viewModel.Cleanup();
            DataContext = null;
        }
    }
}
