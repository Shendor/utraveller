using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Message;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.ViewModel
{
    public abstract class BasePushpinItemViewModel<T> : BaseTimeLineItemViewModel<T> where T : IPushpinItem
    {
        private ICommand showOnMapCommand;

        public BasePushpinItemViewModel(T pushpinItem)
            : base(pushpinItem)
        {
            showOnMapCommand = new ActionCommand(ShowOnMap);
        }


        public ICommand ShowOnMapCommand
        {
            get { return showOnMapCommand; }
        }


        protected abstract void ShowOnMap();


        protected override abstract void OnTimeLineItemDeleted();
    }
}
