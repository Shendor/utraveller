using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UTravellerModel.UTraveller.Model
{
    public class AppProperties
    {
        private static readonly SolidColorBrush DEFAULT_BACKGROUND_COLOR = new SolidColorBrush(Color.FromArgb(255, 38, 42, 50));
        private static readonly SolidColorBrush DEFAULT_MAIN_COLOR = new SolidColorBrush(Color.FromArgb(255, 42, 136, 76));
        private static readonly SolidColorBrush DEFAULT_TEXT_COLOR = new SolidColorBrush(Colors.White);

        public AppProperties()
        {
            Background = DEFAULT_BACKGROUND_COLOR;
            MainColor = DEFAULT_MAIN_COLOR;
            TextColor = DEFAULT_TEXT_COLOR;
            CoverOpacity = 1;
            IsUploadToFacebook = true;
            IsConnectToServerAutomatically = true;
            IsAllowGeoPosition = true;
            Limitation = new Limitation();
        }


        public long Id
        {
            get;
            set;
        }


        public SolidColorBrush Background
        {
            get;
            set;
        }


        public SolidColorBrush MainColor
        {
            get;
            set;
        }


        public SolidColorBrush TextColor
        {
            get;
            set;
        }


        public double CoverOpacity
        {
            get;
            set;
        }


        public double PanelsOpacity
        {
            get;
            set;
        }


        public bool IsLandscapeCover
        {
            get;
            set;
        }

        public bool IsUploadToFacebook
        {
            get;
            set;
        }

        public bool IsConnectToServerAutomatically
        {
            get;
            set;
        }

        public bool IsAllowGeoPosition
        {
            get;
            set;
        }

        public Limitation Limitation
        {
            get;
            set;
        }
    }

    public class Limitation
    {
        public static readonly string CODE1 = "LC7U12RI3THJX";
        public static readonly string CODE2 = "LCUV5T03R93GV";

        public Limitation()
        {
            Code = CODE1;
            TripLimit = 10;
            PhotosLimit = 25;
            MessagesLimit = 25;
            MoneySpendingsLimit = 50;
            RoutesLimit = 1;
        }

        public string Code
        {
            get;
            set;
        }


        public bool IsExtendedColors
        {
            get;
            set;
        }


        public int TripLimit
        {
            get;
            set;
        }


        public int PhotosLimit
        {
            get;
            set;
        }


        public int MessagesLimit
        {
            get;
            set;
        }


        public int MoneySpendingsLimit
        {
            get;
            set;
        }


        public int RoutesLimit
        {
            get;
            set;
        }


        public bool IsRouteEditorEnabled
        {
            get;
            set;
        }


        public override string ToString()
        {
            return string.Format("{0};{1};{2};{3};{4};{5};{6};", IsExtendedColors, TripLimit, PhotosLimit,
                MessagesLimit, MoneySpendingsLimit, RoutesLimit, IsRouteEditorEnabled);
        }
    }
}
