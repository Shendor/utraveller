using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Common.ViewModel
{
    public abstract class BaseListViewModel<T> : List<T>
    {
        public BaseListViewModel()
        {
        }

        public BaseListViewModel(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public Brush MainColor
        {
            get { return App.AppProperties.MainColor; }
        }


        public Brush BackgroundColor
        {
            get { return App.AppProperties.Background; }
        }


        public Brush TextColor
        {
            get { return App.AppProperties.TextColor; }
        }
    }
}
