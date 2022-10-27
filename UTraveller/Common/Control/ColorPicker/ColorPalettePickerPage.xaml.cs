using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Ninject;
using UTraveller.Common.Message;
using System.Windows.Media;

namespace UTraveller.Common.Control.ColorPicker
{
    public partial class ColorPalettePickerPage : PhoneApplicationPage
    {
        public ColorPalettePickerPage()
        {
            InitializeComponent();
        }

        private void PaletteTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (e.OriginalSource is Border)
            {
                SolidColorBrush brush = ((Border)e.OriginalSource).Background as SolidColorBrush;
                ((ColorPickerViewModel)DataContext).ChooseColor(brush.Color);
                NavigationService.GoBack();
            }
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (e.NavigationMode == NavigationMode.Back)
            {
                var viewModel = DataContext as ColorPickerViewModel;
                if (viewModel != null)
                {
                    viewModel.Cleanup();
                    DataContext = null;
                }
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs navigationArgs)
        {
            base.OnNavigatedTo(navigationArgs);
            if (navigationArgs.NavigationMode != NavigationMode.Back)
            {
                var viewModel = App.IocContainer.Get<ColorPickerViewModel>();
                viewModel.Initialize();
                DataContext = viewModel;
            }
        }


        private void CloseButtonTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}