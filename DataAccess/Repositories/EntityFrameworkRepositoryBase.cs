using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using WhiskyClub.DataAccess.Entities;

namespace WhiskyClub.DataAccess.Repositories
{
    public abstract class EntityFrameworkRepositoryBase : IDisposable
    {
        private bool _disposed;
        private WhiskyClubContext DbContext { get; set; }

        protected EntityFrameworkRepositoryBase()
        {
            // This should be changed to be Injectable
            DbContext = new WhiskyClubContext();
        }

        protected TEntity GetOne<TEntity, TKey>(TKey id) where TEntity : class
        {
            var item = DbContext.Set<TEntity>().Find(id);

            if (item != null)
            {
                return item;
            }
            else
            {
                throw new NullReferenceException("Entity not found");
            }
        }

        protected TEntity GetOne<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            var items = GetAll(filter);

            // Relies on FindAll returning an IQueryable that allows 'deferred-loading'
            var item = items.SingleOrDefault();

            if (item != null)
            {
                return item;
            }
            else
            {
                throw new NullReferenceException("Entity not found");
            }
        }

        protected IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return DbContext.Set<TEntity>();
        }

        protected IQueryable<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> filter) where TEntity : class
        {
            return DbContext.Set<TEntity>().Where(filter);
        }

        protected void Insert<TEntity>(TEntity entity) where TEntity : class
        {
            DbContext.Set<TEntity>().Add(entity);
        }

        protected void Update<TEntity>(TEntity entity) where TEntity : class
        {
            // Attach entity (therefore does not need to be loaded from DbContext)
            DbContext.Set<TEntity>().Attach(entity);
            DbContext.Entry(entity).State = EntityState.Modified;
        }

        protected void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            // Attach entity (therefore does not need to be loaded from DbContext)
            DbContext.Set<TEntity>().Attach(entity);
            DbContext.Entry(entity).State = EntityState.Deleted;
            DbContext.Set<TEntity>().Remove(entity);
        }

        protected void CommitChanges()
        {
            DbContext.SaveChanges();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                DbContext = null;
            }

            _disposed = true;
        }

        #endregion
    }
}
