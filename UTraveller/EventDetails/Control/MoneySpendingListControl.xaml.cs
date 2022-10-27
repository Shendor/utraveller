using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.MoneySpendings.Control;
using UTraveller.MoneySpendings.ViewModel;

namespace UTraveller.EventDetails.Control
{
    public partial class MoneySpendingListControl : UserControl
    {
        private MoneySpendingDetailsControl moneySpendingDetails;

        public MoneySpendingListControl()
        {
            InitializeComponent();
        }


        private void MoneySpendingItemTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (moneySpendingDetails == null)
            {
                moneySpendingDetails = new MoneySpendingDetailsControl();
            }
            moneySpendingDetails.Show(((FrameworkElement)sender).Tag as MoneySpendingItemViewModel);
        }
    }
}
