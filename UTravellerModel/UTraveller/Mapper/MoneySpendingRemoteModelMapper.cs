using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;
using UTravellerModel.UTraveller.Model.Remote;

namespace UTravellerModel.UTraveller.Mapper
{
    public class MoneySpendingRemoteModelMapper : IModelMapper<MoneySpending, MoneySpendingRemoteModel>
    {
        public MoneySpending MapEntity(MoneySpendingRemoteModel entity)
        {
            var moneySpending = new MoneySpending();
            moneySpending.RemoteId = entity.Id;
            moneySpending.Amount = (decimal)entity.Amount;
            moneySpending.Date = entity.Date;
            moneySpending.Description = entity.Description;
            moneySpending.Currency = getEnumValueByName<CurrencyType>(entity.Currency);
            moneySpending.MoneySpendingCategory = getEnumValueByName<MoneySpendingCategory>(entity.MoneySpendingCategory);

            return moneySpending;
        }


        public MoneySpendingRemoteModel MapModel(MoneySpending model)
        {
            var moneySpendingRemoteModel = new MoneySpendingRemoteModel();
            moneySpendingRemoteModel.Id = model.RemoteId;
            moneySpendingRemoteModel.Amount = (float)model.Amount;
            moneySpendingRemoteModel.Currency = model.Currency.ToString();
            moneySpendingRemoteModel.Date = new DateTime(model.Date.Ticks, DateTimeKind.Utc);
            moneySpendingRemoteModel.Description = model.Description;
            moneySpendingRemoteModel.MoneySpendingCategory = model.MoneySpendingCategory.ToString();

            return moneySpendingRemoteModel;
        }


        private T getEnumValueByName<T>(string name)
        {
            var values = EnumUtil.GetValues<T>();
            foreach (var value in values)
            {
                if (value.ToString().Equals(name))
                {
                    return value;
                }
            }
            return default(T);
        }


        public static class EnumUtil
        {
            public static IEnumerable<T> GetValues<T>()
            {
                return Enum.GetValues(typeof(T)).Cast<T>();
            }
        }
    }
}
