using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UTravellerModel.UTraveller.Model;

namespace LocalRepositoryImpl.UTraveller.LocalRepository
{
    public abstract class BaseRepository<T> where T : class
    {
        protected LocalDatabase database;

        public BaseRepository(LocalDatabase database)
        {
            if (database == null)
            {
                throw new DatabaseNotFoundException();
            }
            this.database = database;
        }

        public abstract void Insert(T entity);

        public abstract void Delete(T entity);

        public virtual void Update(T entity)
        {
            database.SubmitChanges(ConflictMode.ContinueOnConflict);
        }

        protected void InsertEntityInTable(Table<T> table, T entity)
        {
            table.InsertOnSubmit(entity);
            database.SubmitChanges();
        }

        protected void DeleteEntityFromTable(Table<T> table, T entity)
        {
            table.DeleteOnSubmit(entity);
            database.SubmitChanges();
        }

        public E GetById<E, I>(Table<E> table, I id) where E : class, IBaseEntity<I>
        {
            return (from E e in table
                    where e.Id.Equals(id)
                    select e).FirstOrDefault();
        }
    }
}
