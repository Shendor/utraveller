using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.EventDetails.ViewModel;
using Ninject;

namespace UTraveller.EventDetails.Control
{
    public partial class TripPlanControl : UserControl
    {
        public TripPlanControl()
        {
            InitializeComponent();
        }

        private void PlanItemTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            var item = (Grid)sender;
            var planItem = item.Tag as IPlanItemViewModel;
            if (planItem != null)
            {
                planItem.EditPlanItem();
            }
        }
    }
}
