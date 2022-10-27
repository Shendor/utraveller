using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.EventMap.Messages;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.ViewModel
{
    public abstract class BasePushpinItemInMapViewModel<T> : BasePushpinItemViewModel<T> where T : IPushpinItem
    {
        public BasePushpinItemInMapViewModel(T pushpinItem)
            : base(pushpinItem)
        {
            DeleteFromMapCommand = new ActionCommand(DeleteFromMap);
        }


        public ICommand DeleteFromMapCommand
        {
            get;
            private set;
        }


        public bool IsInPushpin
        {
            get { return DateItem.Coordinate != null; }
        }


        public void UpdateIsInPushpin()
        {
            base.RaisePropertyChanged("IsInPushpin");
        }


        protected abstract void DeleteFromMap();


        protected override abstract void ShowOnMap();


        protected override abstract void OnTimeLineItemDeleted();
    }
}
