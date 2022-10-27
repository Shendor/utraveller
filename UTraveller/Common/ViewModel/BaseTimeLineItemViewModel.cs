using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Resources;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.ViewModel
{
    public abstract class BaseTimeLineItemViewModel<T> : BaseViewModel, IComparable, ITimeLineItem<T> where T : IDateItem
    {
        private ICommand deleteTimeLineItemCommand;
        protected T dateItem;

        public BaseTimeLineItemViewModel(T dateItem)
        {
            this.dateItem = dateItem;
            deleteTimeLineItemCommand = new ActionCommand(OnTimeLineItemDeleted);
        }

        public T DateItem
        {
            get { return dateItem; }
        }


        public DateTime Date
        {
            get { return dateItem.Date; }
        }


        public string ShortFormattedDate
        {
            get { return Date.ToString(AppResources.Short_No_Week_Name_Date_Format, App.CurrentCulture); }
        }


        public string FullFormattedDate
        {
            get { return Date.ToString(AppResources.Full_Date_Format, App.CurrentCulture); }
        }


        public ICommand DeleteTimeLineItemCommand
        {
            get { return deleteTimeLineItemCommand; }
        }


        protected abstract void OnTimeLineItemDeleted();


        public int CompareTo(object other)
        {
            return -Date.CompareTo(((ITimeLineItem<IDateItem>)other).Date);
        }
    }
}
