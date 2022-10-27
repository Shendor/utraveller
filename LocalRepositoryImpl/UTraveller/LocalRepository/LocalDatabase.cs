using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Entity;

namespace LocalRepositoryImpl.UTraveller.LocalRepository
{
    public class LocalDatabase : DataContext
    {
        public LocalDatabase(string connectionString)
            : base(connectionString)
        {
        }

        public Table<UserEntity> Users;

        public Table<AppPropertiesEntity> Properties;

        public Table<AuthenticationRegistryEntity> AuthenticationRegistry;

        public Table<EventEntity> Events;

        public Table<PhotoEntity> Photos;

        public Table<RouteEntity> Routes;

        public Table<RoutePushpinEntity> RoutePushpins;

        public Table<MoneySpendingEntity> MoneySpending;

        public Table<MessageEntity> Messages;

        public Table<TripPlanEntity> TripPlans;

    }
}
