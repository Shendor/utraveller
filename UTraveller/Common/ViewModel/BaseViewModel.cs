using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UTraveller.Common.Message;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.ViewModel
{
    public abstract class BaseViewModel : ViewModelBase
    {
        public const string BACKGROUND_COLOR_KEY = "background_color";
        public const string MAIN_COLOR_KEY = "main_color";
        public const string TEXT_COLOR_KEY = "text_color";

        public AppProperties AppProperties
        {
            get { return App.AppProperties; }
        }


        public Brush BackgroundColor
        {
            get { return App.AppProperties.Background; }
        }


        public Brush MainColor
        {
            get { return App.AppProperties.MainColor; }
        }


        public Brush TextColor
        {
            get { return App.AppProperties.TextColor; }
        }


        public void RefreshColors()
        {
            RaisePropertyChanged("BackgroundColor");
            RaisePropertyChanged("MainColor");
            RaisePropertyChanged("TextColor");
        }
    }
}
