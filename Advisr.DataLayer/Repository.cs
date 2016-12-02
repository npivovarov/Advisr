using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Advisr.DataLayer
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public DbContext context;
        private DbSet<T> dbSet;

        public Repository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<T>();
        }

        public T GetById(int id)
        {
            return this.dbSet.Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return this.dbSet;
        }

        public void Insert(T entity)
        {
            this.dbSet.Add(entity);
        }

        public void InsertRange(IEnumerable<T> entities)
        {
            this.dbSet.AddRange(entities);
        }

        public void Edit(T entity)
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
        }

        public void Delete(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                context.Entry(entity).State = EntityState.Deleted;
            }
        }
        
        public void Detached(T entity)
        {
            context.Entry(entity).State = EntityState.Detached;
        }

    }
}
