using System.Linq.Expressions;
using CatagoloAPI.Context;
using CatagoloAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<T> GetAll()  => _context.Set<T>().ToList();

    public T? Get(Expression<Func<T, bool>> predicate) => _context.Set<T>().FirstOrDefault(predicate);

    public T Create(T item)
    {
        _context.Set<T>().Add(item);
        _context.SaveChanges();
        
        return item;
    }

    public T Update(T item)
    {
        _context.Set<T>().Update(item);
        _context.SaveChanges();
        
        return item;
    }

    public T Delete(T item)
    {
        _context.Set<T>().Remove(item);
        _context.SaveChanges();
        
        return item;
    }
}