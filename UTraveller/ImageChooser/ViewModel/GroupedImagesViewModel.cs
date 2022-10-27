using GalaSoft.MvvmLight.Messaging;
using Microsoft.Expression.Interactivity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.ImageChooser.Model;
using UTraveller.ImageChooser.ViewModel;
using UTraveller.Service.Api;

namespace UTraveller.ImageChooser.ViewModel
{
    public class GroupedImagesViewModel : PhotoListViewModel
    {
        public GroupedImagesViewModel(IGrouping<string, CheckedImageModel> grouping, INavigationService navigationService,
            IMessenger messenger)
            : base(grouping, navigationService, messenger)
        {
            Month = grouping.Key;
        }

        public string Month
        {
            protected set;
            get;
        }

        public override bool Equals(object obj)
        {
            if (obj is GroupedImagesViewModel)
            {
                return Month.Equals(((GroupedImagesViewModel)obj).Month);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Month.GetHashCode();
        }

        public override string ToString()
        {
            return Month;
        }
    }
}
