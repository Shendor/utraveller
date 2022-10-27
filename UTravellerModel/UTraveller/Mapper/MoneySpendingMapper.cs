using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Model;

namespace UTravellerModel.UTraveller.Mapper
{
    public class MoneySpendingMapper : IModelMapper<MoneySpending, MoneySpendingEntity>
    {
        public MoneySpending MapEntity(MoneySpendingEntity entity)
        {
            var moneySpending = new MoneySpending();
            moneySpending.Id = entity.Id;
            moneySpending.RemoteId = entity.RemoteId;
            moneySpending.IsSync = entity.IsSync;
            moneySpending.MoneySpendingCategory = entity.MoneySpendingCategory;
            moneySpending.Amount = entity.Amount;
            moneySpending.Description = entity.Description;
            moneySpending.Currency = entity.Currency;
            moneySpending.Date = entity.Date;
            moneySpending.IsDeleted = entity.IsDeleted;

            return moneySpending;
        }


        public MoneySpendingEntity MapModel(MoneySpending model)
        {
            var entity = new MoneySpendingEntity();
            entity.Id = model.Id;
            entity.RemoteId = model.RemoteId;
            entity.IsSync = model.IsSync;
            entity.MoneySpendingCategory = model.MoneySpendingCategory;
            entity.Amount = model.Amount;
            entity.Description = model.Description;
            entity.Currency = model.Currency;
            entity.Date = model.Date;
            entity.IsDeleted = model.IsDeleted;

            return entity;
        }
    }
}
