using System.Linq.Expressions;

namespace CatagoloAPI.Repositories.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    T Create(T item);
    T Update(T item);
    T Delete(T item);
}