using System;
using System.Linq;
using System.Linq.Expressions;
using WhiskyClub.DataAccess.Entities;

namespace WhiskyClub.DataAccess.Reposistories
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
