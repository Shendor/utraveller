using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.Sync.ViewModel
{
    public class SyncEntityViewModel : BaseViewModel
    {

        public SyncEntityViewModel(BaseModel entity)
        {
            Entity = entity;
        }


        public BaseModel Entity
        {
            get;
            set;
        }

        public string Category
        {
            get
            {
                if (Entity is MoneySpending)
                {
                    return MoneySpendingCategoryUtil.GetMoneySpendingCategoryName(((MoneySpending)Entity).MoneySpendingCategory);
                }
                return null;
            }
        }


        public string Amount
        {
            get
            {
                if (Entity is MoneySpending)
                {
                    return ((MoneySpending)Entity).Amount.ToString("0") + " " + CurrencyUtil.GetCurrencySymbol(((MoneySpending)Entity).Currency);
                }
                return null;
            }
        }


        public SyncType SyncType
        {
            get
            {
                if (Entity.IsDeleted)
                {
                    return SyncType.DELETE;
                }
                else if (Entity.RemoteId > 0 && Entity.IsSync == false)
                {
                    return SyncType.UPDATE;
                }
                else
                {
                    return SyncType.ADD;
                }
            }
        }
    }
}
