using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Repositories;

public class ProdutoRepository :  IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public IEnumerable<Produto> GetAll() => _context.Produtos.AsNoTracking().ToList(); 

    public Produto GetById(int id) => _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

    public Produto Create(Produto produto)
    {
        if(produto == null) throw new ArgumentNullException(nameof(produto));
        
        _context.Produtos.Add(produto);
        _context.SaveChanges();
        
        return produto;
    }

    public Produto Update(Produto produto)
    {
        if(produto == null) throw new ArgumentNullException(nameof(produto));
        
        _context.Produtos.Entry(produto).State = EntityState.Modified;
        _context.SaveChanges();
        
        return produto;
    }

    public Produto Delete(int id)
    {
        var produto =  _context.Produtos.Find(id);
        
        if(produto == null) throw new ArgumentNullException(nameof(produto));
        
        _context.Produtos.Remove(produto);
        _context.SaveChanges();
        
        return produto;
    }
}