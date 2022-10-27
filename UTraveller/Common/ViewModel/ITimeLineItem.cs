using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.ViewModel
{
    public interface ITimeLineItem<out T> where T : IDateItem
    {
        DateTime Date
        {
            get;
        }

        T DateItem
        {
            get;
        }
    }
}
