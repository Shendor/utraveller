using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Sync.ViewModel
{
    public class GroupedSyncItemsViewModel : BaseListViewModel<SyncEntityViewModel>
    {
        private static readonly BitmapImage ADD_ICON = new BitmapImage(new Uri("/Assets/AppBar/add.png", UriKind.Relative));
        private static readonly BitmapImage UPDATE_ICON = new BitmapImage(new Uri("/Assets/Icons/update.png", UriKind.Relative));
        private static readonly BitmapImage DELETE_ICON = new BitmapImage(new Uri("/Assets/AppBar/close.png", UriKind.Relative));

        public GroupedSyncItemsViewModel(IGrouping<SyncType, SyncEntityViewModel> itemsGroup)
            : base(itemsGroup)
        {
            SyncType = itemsGroup.Key;
        }


        public SyncType SyncType
        {
            protected set;
            get;
        }


        public string SyncTypeText
        {
            get
            {
                if (SyncType == SyncType.DELETE)
                {
                    return "items to delete";
                }
                else if (SyncType == SyncType.UPDATE)
                {
                    return "items to update";
                }
                else
                {
                    return "items to add";
                }

            }
        }


        public BitmapImage SyncTypeImage
        {
            get
            {
                if (SyncType == SyncType.DELETE)
                {
                    return DELETE_ICON;
                }
                else if (SyncType == SyncType.UPDATE)
                {
                    return UPDATE_ICON;
                }
                else
                {
                    return ADD_ICON;
                }
            }
        }


        public override bool Equals(object obj)
        {
            if (obj is GroupedSyncItemsViewModel)
            {
                return SyncType.Equals(((GroupedSyncItemsViewModel)obj).SyncType);
            }
            return false;
        }


        public override int GetHashCode()
        {
            return SyncType.GetHashCode();
        }


        public override string ToString()
        {
            return SyncType.ToString();
        }
    }

    public enum SyncType
    {
        ADD, UPDATE, DELETE
    }
}
