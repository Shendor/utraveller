using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using UTraveller.SkyDrive.ViewModel;
using Ninject;
using UTraveller.SkyDrive.Message;
using UTraveller.Routes.ViewModel;
using UTraveller.Common.Message;
using UTraveller.Service.Api;
using Ninject;
using UTraveller.Common.Control;

namespace UTraveller.SkyDrive
{
    public partial class SkyDriveExplorerPage : BasePhoneApplicationPage
    {
        private ITaskExecutionManager taskExecutionManager;
        private SkyDriveExplorerViewModel viewModel;

        public SkyDriveExplorerPage()
        {
            InitializeComponent();
            taskExecutionManager = App.IocContainer.Get<ITaskExecutionManager>();
        }


        public override void Activate()
        {
            base.Activate();
            if (viewModel != null)
            {
                viewModel.Initialize();
            }
        }


        //protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        //{
        //    if (taskExecutionManager.IsTaskRunned())
        //    {
        //        e.Cancel = true;
        //    }
        //    base.OnBackKeyPress(e);
        //}

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                ((SkyDriveExplorerViewModel)DataContext).Cleanup();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode != NavigationMode.Back)
            {
                var queryParmeter = UTraveller.Service.Implementation.NavigationService.PARAMETER_NAME;
                var viewModels = CreateViewModelsDictionary();
                viewModel = viewModels[NavigationContext.QueryString[queryParmeter]] as SkyDriveExplorerViewModel;

                if (viewModel != null)
                {
                    DataContext = viewModel;
                    viewModel.Initialize();
                }
            }
        }


        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1)
            {
                var item = (BaseSkyDriveItemViewModel)e.AddedItems[0];
                App.Messenger.Send<SkyDriveItemChangedMessage>(new SkyDriveItemChangedMessage(item));
            }
        }


        private static IDictionary<string, SkyDriveExplorerViewModel> CreateViewModelsDictionary()
        {
            IDictionary<string, SkyDriveExplorerViewModel> viewModels = new Dictionary<string, SkyDriveExplorerViewModel>();
            viewModels.Add(typeof(RoutesViewModel).ToString(), App.IocContainer.Get<RouteSkyDriveExplorerViewModel>());
            return viewModels;
        }


        private void CloseButtonClick(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Close();
        }
    }
}