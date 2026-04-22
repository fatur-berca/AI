using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TOM.EntitiesDAL.EDMX;

namespace TOM.EntitiesDAL
{
    public class TOMGenericRepository<TEntity> : ITOMGenericRepository<TEntity> where TEntity : class
    {
        internal TOMContextDB Context;
        internal DbSet<TEntity> DbSet;
        internal DbContextTransaction ContextTransaction = null;

        public void DetachAll()
        {
            foreach (DbEntityEntry dbEntityEntry in Context.ChangeTracker.Entries())
            {
                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }

        public TOMGenericRepository(TOMContextDB contextEntities)
        {
            Context = contextEntities;
            DbSet = Context.Set<TEntity>();
        }

        public int Save()
        {
            try
            {
                Context.SaveChanges();
            }
            //catch (Exception e)
            //{
            //    var message = e;

            //}
            catch (DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}", validationErrors.Entry.Entity, validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            return 0;
        }

        public void Save(string controller, string userid)
        {
            if (Context.SaveChanges() > 0)
            {
                Context.SaveChanges();
            }
        }

        private bool disposed;

        public bool AllowLazyLoading { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public void Update(TEntity entityToUpdate)
        {
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public string DoUpdate(TEntity entityToUpdate)
        {
            try
            {
                if (entityToUpdate != null)
                {
                    if (!DbSet.Local.Contains(entityToUpdate))
                        DbSet.Attach(entityToUpdate);
                    Context.Entry(entityToUpdate).State = EntityState.Modified;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return "";
        }

        public void InsertOrUpdate(TEntity entity)
        {
            if (!Exists(entity))
                Insert(entity);
            else
                Update(entity);
        }

        public bool Exists(TEntity entity)
        {
            var objContext = ((IObjectContextAdapter)Context).ObjectContext;
            var objSet = objContext.CreateObjectSet<TEntity>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);
            return (exists);
        }

        public void DeleteAll()
        {
            DbSet.RemoveRange(DbSet);
        }

        public void Delete(object id)
        {
            var entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public TEntity GetByID(object id)
        {
            return DbSet.Find(id);
        }

        public TEntity GetByName(object name)
        {
            return DbSet.Find(name);
        }

        public TEntity GetByID(params object[] keyValues)
        {
            return DbSet.Find(keyValues);
        }
        public IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public TEntity GetOne(Expression<Func<TEntity, bool>> filter = null)
        {
            if (filter != null)
            {
                return DbSet.AsNoTracking().Where(filter).FirstOrDefault();
            }

            return DbSet.AsNoTracking().FirstOrDefault();
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return orderBy != null ? orderBy(query).ToList() : query.ToList();
        }

        public IEnumerable<TEntity> Get(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            var res = orderBy != null ? orderBy(query) : query;
            return res.Skip(pageIndex).Take(pageSize);
        }

        public IEnumerable<TEntity> GetAllPagination(Expression<Func<TEntity, bool>> filter = null, int skip = 0, int take = 0, string orderBy = "ID", string orderType = "asc")
        {
            IEnumerable<TEntity> directResult = null;
            try
            {
                var order = GetOrderBy(orderBy, orderType);
                if (filter != null)
                {
                    directResult = order(DbSet.Where(filter)).Skip(skip).Take(take);
                }
                else
                {
                    directResult = order(DbSet).Skip(skip).Take(take);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return directResult;
        }

        public static Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> GetOrderBy(string orderColumn, string orderType)
        {
            Type typeQueryable = typeof(IQueryable<TEntity>);
            ParameterExpression argQueryable = Expression.Parameter(typeQueryable, "p");
            var outerExpression = Expression.Lambda(argQueryable, argQueryable);
            string[] props = orderColumn.Split('.');
            IQueryable<TEntity> query = new List<TEntity>().AsQueryable<TEntity>();
            Type type = typeof(TEntity);
            ParameterExpression arg = Expression.Parameter(type, "x");

            Expression expr = arg;
            foreach (string prop in props)
            {
                PropertyInfo pi = type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            LambdaExpression lambda = Expression.Lambda(expr, arg);
            string methodName = orderType == "asc" ? "OrderBy" : "OrderByDescending";

            MethodCallExpression resultExp =
                Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(TEntity), type }, outerExpression.Body, Expression.Quote(lambda));
            var finalLambda = Expression.Lambda(resultExp, argQueryable);
            return (Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>)finalLambda.Compile();
        }

        public long Count(Expression<Func<TEntity, bool>> filter = null)
        {
            long directResult;
            try
            {
                if (filter != null)
                {
                    directResult = DbSet.Where(filter).LongCount();
                }
                else
                {
                    directResult = DbSet.LongCount();
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return directResult;
        }

        public void BeginTransaction(System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted | System.Data.IsolationLevel.Snapshot)
        {
            ContextTransaction = Context.Database.BeginTransaction();
        }

        public void EndTransaction(bool commit = true)
        {
            if (ContextTransaction != null)
            {
                try
                {
                    if (commit)
                        ContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    ContextTransaction.Rollback();
                    throw ex;
                }
                if (commit == false)
                    ContextTransaction.Rollback();
            }
        }
    }
}
