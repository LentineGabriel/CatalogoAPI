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
    
    public async Task<IEnumerable<T>> GetAllAsync()  => await _context.Set<T>().AsNoTracking().ToListAsync();

    public async Task<T?> GetIdAsync(Expression<Func<T, bool>> predicate) => await _context.Set<T>().FirstOrDefaultAsync(predicate);

    public T Create(T item)
    {
        _context.Set<T>().Add(item);
        // _context.SaveChanges();
        
        return item;
    }

    public T Update(T item)
    {
        _context.Set<T>().Update(item);
        // _context.SaveChanges();
        
        return item;
    }

    public T Delete(T item)
    {
        _context.Set<T>().Remove(item);
        // _context.SaveChanges();
        
        return item;
    }
}