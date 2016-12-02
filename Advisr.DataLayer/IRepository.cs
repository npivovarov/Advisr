using System.Collections.Generic;
using System.Linq;

namespace Advisr.DataLayer
{
    public interface IRepository<T>
    {
        T GetById(int id);

        IQueryable<T> GetAll();

        void Edit(T entity);

        void Insert(T entity);

        void InsertRange(IEnumerable<T> entities);

        void Delete(T entity);

        void Delete(IEnumerable<T> entities);

        void Detached(T entity);
    }
}
