using System.Linq.Expressions;

namespace CatagoloAPI.Repositories.Interfaces;

public interface IRepository<T>
{
    IEnumerable<T> GetAll();
    T? Get(Expression<Func<T, bool>> predicate);
    T Create(T item);
    T Update(T item);
    T Delete(T item);
}