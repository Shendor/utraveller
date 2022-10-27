using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Windows.Media;
using UTravellerModel.UTraveller.Converter;
using UTraveller.Common.ViewModel;

namespace UTraveller.Common.Control
{
    public partial class SplashScreen : PhoneApplicationPage
    {
        public SplashScreen()
        {
            InitializeComponent();

            if (IsolatedStorageSettings.ApplicationSettings.Contains(BaseViewModel.BACKGROUND_COLOR_KEY))
            {
                var backgroundColor = HexColorConverter.GetColorFromHex(IsolatedStorageSettings.ApplicationSettings[BaseViewModel.BACKGROUND_COLOR_KEY].ToString());
                layoutRoot.Background = new SolidColorBrush(backgroundColor);
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains(BaseViewModel.MAIN_COLOR_KEY))
            {
                var mainColor = HexColorConverter.GetColorFromHex(IsolatedStorageSettings.ApplicationSettings[BaseViewModel.MAIN_COLOR_KEY].ToString());
                logoBorder.Background = new SolidColorBrush(mainColor);
                //progressBar.Foreground = new SolidColorBrush(mainColor);
            }

            /*if (IsolatedStorageSettings.ApplicationSettings.Contains(BaseViewModel.TEXT_COLOR_KEY))
            {
                var textColor = HexColorConverter.GetColorFromHex(IsolatedStorageSettings.ApplicationSettings[BaseViewModel.TEXT_COLOR_KEY].ToString());
                waitText.Foreground = new SolidColorBrush(textColor);
            }*/
        }
    }
}