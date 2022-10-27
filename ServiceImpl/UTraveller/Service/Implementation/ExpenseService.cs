using UTraveller.Service.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;
using RepositoryApi.UTraveller.Repository.Api;
using UTravellerModel.UTraveller.Entity;
using UTravellerModel.UTraveller.Mapper;
using UTraveller.Service.Api.Remote;
using UTraveller.Service.Implementation.Internal;
using ServiceApi.UTraveller.Service.Exceptions;

namespace UTraveller.Service.Implementation
{
    public class ExpenseService : BaseCacheableEntityService, IExpenseService
    {
        private IMoneySpendingRepository moneySpendingRepository;
        private IModelMapper<MoneySpending, MoneySpendingEntity> moneySpendingMapper;
        private IAppPropertiesService appPropertiesService;

        public ExpenseService(IMoneySpendingRepository moneySpendingRepository,
             IModelMapper<MoneySpending, MoneySpendingEntity> moneySpendingMapper,
             IAppPropertiesService appPropertiesService)
        {
            this.moneySpendingRepository = moneySpendingRepository;
            this.moneySpendingMapper = moneySpendingMapper;
            this.appPropertiesService = appPropertiesService;
        }


        public IEnumerable<MoneySpending> GetExpenses(Event e)
        {
            return GetExpensesLocally(e);
        }


        public IDictionary<CurrencyType, decimal> GetTotalSpentMoneyForEvent(Event e)
        {
            var moneySpendings = new Dictionary<CurrencyType, decimal>();
            var moneySpendingEntities = moneySpendingRepository.GetMoneySpendingsForEvent(e.Id);

            foreach (var moneySpendingEntity in moneySpendingEntities)
            {
                if (!moneySpendings.ContainsKey(moneySpendingEntity.Currency))
                {
                    moneySpendings[moneySpendingEntity.Currency] = 0;
                }
                moneySpendings[moneySpendingEntity.Currency] += moneySpendingEntity.Amount;
            }

            return moneySpendings;
        }


        public void AddExpense(MoneySpending moneySpending, Event e)
        {
            var moneySpendingsQuantity = GetExpensesQuantity(e.Id);
            if (!IsLimitExceeded(e, moneySpendingsQuantity + 1))
            {
                AddExpenseLocally(moneySpending, e);
            }
        }


        public void DeleteExpense(MoneySpending moneySpending, Event e)
        {
            var entity = moneySpendingRepository.GetById(moneySpending.Id);
            if (entity != null && entity.EventId == e.Id)
            {
                moneySpendingRepository.Delete(entity);
            }
        }


        private void AddExpenseLocally(MoneySpending moneySpending, Event e)
        {
            var entity = moneySpendingMapper.MapModel(moneySpending);
            entity.EventId = e.Id;
            moneySpendingRepository.Insert(entity);
            moneySpending.Id = entity.Id;
        }


        private ICollection<MoneySpending> GetExpensesLocally(Event e)
        {
            var moneySpendings = new List<MoneySpending>();
            foreach (var moneySpending in moneySpendingRepository.GetMoneySpendingsForEvent(e.Id))
            {
                moneySpendings.Add(moneySpendingMapper.MapEntity(moneySpending));
            }
            return moneySpendings;
        }


        public int GetExpensesQuantity(long eventId)
        {
            return moneySpendingRepository.GetMoneySpendingsQuantity(eventId);
        }


        private bool IsLimitExceeded(Event e, int expenseQuantity)
        {
            var properties = appPropertiesService.GetPropertiesForUser(e.UserId);
            if (properties.Limitation.MoneySpendingsLimit < expenseQuantity)
            {
                throw new LimitExceedException(properties.Limitation.MoneySpendingsLimit, "Expenses");
            }
            else
            {
                return false;
            }
        }


        public Type GetEntityType()
        {
            return typeof(MoneySpending);
        }

    }
}
