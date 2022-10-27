using GalaSoft.MvvmLight;
using Microsoft.Expression.Interactivity.Core;
using ServiceApi.UTraveller.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UTraveller.Common.Control;
using UTraveller.Common.Message;
using UTraveller.Common.Util;
using UTraveller.Common.ViewModel;
using UTraveller.MoneySpendings.Message;
using UTraveller.MoneySpendings.Model;
using UTraveller.Resources;
using UTraveller.Service.Api;
using UTravellerModel.UTraveller.Model;

namespace UTraveller.MoneySpendings.ViewModel
{
    public class MoneySpendingViewModel : BaseViewModel
    {
        private IExpenseService moneySpendingService;
        private NotificationService notificationService;
        private Event currentEvent;

        public MoneySpendingViewModel(IExpenseService moneySpendingService, NotificationService notificationService)
        {
            this.moneySpendingService = moneySpendingService;
            this.notificationService = notificationService;

            AddSpendingCommand = new ActionCommand(AddSpending);

            MessengerInstance.Register<EventSelectionChangedMessage>(this, OnEventChanged);

            Date = DateTime.Now;
            Time = DateTime.Now;
        }


        public void Initialize()
        {
            Date = DateTime.Now;
            Time = DateTime.Now;
            RaisePropertyChanged("Date");
            RaisePropertyChanged("Time");

            InitializeMoneySpendingCategories();
            InitializeCurrenices();
        }


        public override void Cleanup()
        {
            Categories = null;
            Currencies = null;
            SelectedCategory = null;
            SelectedCurrency = null;
            Description = null;
            Amount = 0;
        }


        public ICommand AddSpendingCommand
        {
            get;
            private set;
        }


        public ICollection<MoneySpendingCategoryModel> Categories
        {
            get;
            private set;
        }


        public ICollection<CurrencyModel> Currencies
        {
            get;
            private set;
        }


        public MoneySpendingCategoryModel SelectedCategory
        {
            get;
            set;
        }


        public CurrencyModel SelectedCurrency
        {
            get;
            set;
        }


        public decimal Amount
        {
            get;
            set;
        }


        public DateTime Date
        {
            get;
            set;
        }


        public DateTime Time
        {
            get;
            set;
        }


        public string Description
        {
            get;
            set;
        }


        private void InitializeCurrenices()
        {
            Currencies = new SortedSet<CurrencyModel>();

            foreach (var enumValue in Enum.GetValues(typeof(CurrencyType)))
            {
                var currency = (CurrencyType)enumValue;
                Currencies.Add(new CurrencyModel(currency, CurrencyUtil.GetCurrencyName(currency),
                    CurrencyUtil.GetCurrencySymbol(currency)));
            }
            RaisePropertyChanged("Currencies");
            InitializeDefaultSelectedCurrency();
        }


        private void InitializeDefaultSelectedCurrency()
        {
            string selectedCurrencyCode = null;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(App.SELECTED_CURRENCY))
            {
                selectedCurrencyCode = IsolatedStorageSettings.ApplicationSettings[App.SELECTED_CURRENCY].ToString();
            }
            SelectedCurrency = Currencies.FirstOrDefault((c) => c.Currency.ToString().Equals(selectedCurrencyCode));
        }


        private void InitializeMoneySpendingCategories()
        {
            Categories = new SortedSet<MoneySpendingCategoryModel>();
            foreach (var enumValue in Enum.GetValues(typeof(MoneySpendingCategory)))
            {
                var category = (MoneySpendingCategory)enumValue;
                Categories.Add(new MoneySpendingCategoryModel(category,
                    MoneySpendingCategoryUtil.GetMoneySpendingCategoryName(category)));
            }
            RaisePropertyChanged("Categories");
            InitializeDefaultSelectedExpenseCategory();
        }

        private void InitializeDefaultSelectedExpenseCategory()
        {
            string selectedCategory = null;
            if (IsolatedStorageSettings.ApplicationSettings.Contains(App.SELECTED_EXPENSE_CATEGORY))
            {
                selectedCategory =
                    IsolatedStorageSettings.ApplicationSettings[App.SELECTED_EXPENSE_CATEGORY].ToString();
            }
            SelectedCategory = Categories.FirstOrDefault((c) => c.Category.ToString().Equals(selectedCategory));
        }


        private void AddSpending()
        {
            var moneySpending = new MoneySpending();
            moneySpending.Date = new DateTime(Date.Year, Date.Month, Date.Day, Time.Hour, Time.Minute, 0);
            moneySpending.MoneySpendingCategory = SelectedCategory.Category;
            moneySpending.Currency = SelectedCurrency.Currency;
            moneySpending.Amount = Amount;
            moneySpending.Description = Description;

            try
            {
                if (currentEvent != null)
                {
                    SaveSelectedValues();
                    moneySpendingService.AddExpense(moneySpending, currentEvent);
                }
                MessengerInstance.Send<MoneySpendingAddedMessage>(new MoneySpendingAddedMessage(moneySpending));
            }
            catch (LimitExceedException ex)
            {
                notificationService.Show(string.Format(AppResources.Limit_Exceeded, AppResources.MoneySpendings, ex.Limit));
            }
        }

        private void SaveSelectedValues()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains(App.SELECTED_CURRENCY))
            {
                IsolatedStorageSettings.ApplicationSettings[App.SELECTED_CURRENCY] = SelectedCurrency.Currency.ToString();
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add(App.SELECTED_CURRENCY, SelectedCurrency.Currency.ToString());
            }
            if (IsolatedStorageSettings.ApplicationSettings.Contains(App.SELECTED_EXPENSE_CATEGORY))
            {
                IsolatedStorageSettings.ApplicationSettings[App.SELECTED_EXPENSE_CATEGORY] = SelectedCategory.Category.ToString();
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add(App.SELECTED_EXPENSE_CATEGORY, SelectedCategory.Category.ToString());
            }
            IsolatedStorageSettings.ApplicationSettings.Save();
        }


        private void OnEventChanged(EventSelectionChangedMessage message)
        {
            currentEvent = message.Object;
        }

    }
}
