using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTraveller.Common.Util;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.MoneySpendings.Model
{
    public class MoneySpendingCategoryModel : IComparable<MoneySpendingCategoryModel>
    {
        public MoneySpendingCategoryModel(MoneySpendingCategory category, string name)
        {
            Category = category;
            Name = name;
        }


        public MoneySpendingCategory Category
        {
            get;
            private set;
        }


        public string Name
        {
            get;
            private set;
        }

        public Uri Icon
        {
            get { return MoneySpendingCategoryUtil.GetIcon(Category); }
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MoneySpendingCategoryModel))
            {
                return false;
            }

            return ((MoneySpendingCategoryModel)obj).Category == this.Category;
        }

        public override int GetHashCode()
        {
            return this.Category.GetHashCode();
        }

        public int CompareTo(MoneySpendingCategoryModel other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
