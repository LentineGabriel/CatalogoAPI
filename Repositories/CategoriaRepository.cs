using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Repositories;

public class CategoriaRepository : ICategoriaRepository
{
    private readonly AppDbContext _context;

    public CategoriaRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Categoria> GetAll() => _context.Categorias.AsNoTracking().ToList();
    
    public IEnumerable<Categoria> GetAllWithProducts() => _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
    
    public Categoria GetById(int id) => _context.Categorias.FirstOrDefault(c => c.CategoriaId == id);

    public Categoria Create(Categoria categoria)
    {
        if (categoria == null) throw new ArgumentNullException(nameof(categoria));

        _context.Categorias.Add(categoria);
        _context.SaveChanges();
        
        return categoria;
    }

    public Categoria Update(Categoria categoria)
    {
        if(categoria == null) throw new ArgumentNullException(nameof(categoria));

        _context.Entry(categoria).State = EntityState.Modified;
        _context.SaveChanges();
        
        return categoria;
    }

    public Categoria Delete(int id)
    {
        var categoria = _context.Categorias.Find(id);
        
        if(categoria == null) throw new ArgumentNullException(nameof(categoria));
        
        _context.Categorias.Remove(categoria);
        _context.SaveChanges();
        
        return categoria;
    }
}