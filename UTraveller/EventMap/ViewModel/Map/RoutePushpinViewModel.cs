using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UTraveller.Common.ViewModel;
using UTraveller.EventMap.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventMap.ViewModel
{
    public class RoutePushpinViewModel : BasePushpinViewModel
    {
        public RoutePushpinViewModel(RoutePushpin pushpin)
        {
            Pushpin = pushpin;
            CreatePlanItemCommand = new ActionCommand(CreatePlanItem);
        }


        public ICommand CreatePlanItemCommand
        {
            get;
            private set;
        }


        public RoutePushpin Pushpin
        {
            get;
            set;
        }


        public BitmapImage Thumbnail
        {
            get { return Pushpin.Thumbnail; }
        }


        public Brush Color
        {
            get { return new SolidColorBrush(Pushpin.Color); }
        }


        private void CreatePlanItem()
        {
            MessengerInstance.Send<LaunchCreatePlanItemFromRoutePushpinPageMessage>(new LaunchCreatePlanItemFromRoutePushpinPageMessage(Pushpin));
        }
    }
}
