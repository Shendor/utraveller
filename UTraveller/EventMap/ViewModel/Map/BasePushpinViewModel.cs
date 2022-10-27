using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UTraveller.Common.ViewModel;

namespace UTraveller.EventMap.ViewModel
{
    public abstract class BasePushpinViewModel : BaseViewModel
    {
        private Visibility visibility;

        public Visibility Visibility
        {
            get { return visibility; }
            set
            {
                visibility = value;
                RaisePropertyChanged("Visibility");
            }
        }
    }
}
