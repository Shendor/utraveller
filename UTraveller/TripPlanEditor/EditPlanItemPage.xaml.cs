using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.Common.Control;
using UTraveller.TripPlanEditor.ViewModel;
using UTraveller.TripPlanEditor.Messages;
using Ninject;
using UTraveller.TripPlanEditor.Model;
using UTraveller.TripPlanEditor.Control;

namespace UTraveller.TripPlanEditor
{
    public partial class EditPlanItemPage : BasePhoneApplicationPage
    {
        private EditPlanItemViewModel viewModel;

        public EditPlanItemPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                App.Messenger.Register<EditPlanItemTypeChangedMessage>(typeof(EditPlanItemPage), OnEditPlanItemTypeChanged);
                DataContext = viewModel = App.IocContainer.Get<EditPlanItemViewModel>();
                viewModel.Initialize();
                App.Messenger.Register<PlanItemSavedMessage>(typeof(EditPlanItemPage), OnPlanItemSaved);
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                App.Messenger.Unregister<PlanItemSavedMessage>(typeof(EditPlanItemPage));
                App.Messenger.Unregister<EditPlanItemTypeChangedMessage>(typeof(EditPlanItemPage));
                viewModel.Cleanup();
            }
        }


        private void CancelButtonClick(object sender, EventArgs e)
        {
            Close();
        }


        private void OnPlanItemSaved(PlanItemSavedMessage message)
        {
            Close();
        }


        private void OnEditPlanItemTypeChanged(EditPlanItemTypeChangedMessage message)
        {
            content.Children.Clear();
            UserControl editControl = null;
            if (message.EditPlanItemType == EditPlanItemType.Other)
            {
                editControl = new EditPlanItemControl();
            }
            else if (message.EditPlanItemType == EditPlanItemType.Rent)
            {
                editControl = new EditRentPlanItemControl();
            }
            else
            {
                editControl = new EditTransportPlanItemControl();
            }
            editControl.DataContext = viewModel.PlanItemViewModel;
            content.Children.Add(editControl);
        }
    }
}