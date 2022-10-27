using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using UTraveller.Common.Message;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.EventDetails.ViewModel
{
    public class PhotoViewModel : BasePushpinItemViewModel<Photo>, IDescriptionedTimeLineItem
    {
        public PhotoViewModel(Photo photo)
            : base(photo)
        {
        }


        public void UpdateDescription()
        {
            RaisePropertyChanged("DateItem.Description");
        }


        protected override void ShowOnMap()
        {
            
        }


        protected override void OnTimeLineItemDeleted()
        {
            MessengerInstance.Send<DeletedEventPhotoMessage>(new DeletedEventPhotoMessage(DateItem));
        }
    }
}
