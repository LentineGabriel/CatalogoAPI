using CatagoloAPI.Context;
using CatagoloAPI.Repositories.Interfaces;

namespace CatagoloAPI.Repositories;

public class UnitOfWork : IUnitOfWork
{
    #region Props/Ctor
    private IProdutoRepository? _produtoRepo;
    private ICategoriaRepository? _categoriaRepo;
    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }
    #endregion

    #region Methods
    public IProdutoRepository ProdutoRepository
    {
        get => _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context);
    }

    public ICategoriaRepository CategoriaRepository
    {
        get => _categoriaRepo = _categoriaRepo ?? new CategoriaRepository(_context);
    }

    public async Task CommitAsync() => await _context.SaveChangesAsync();

    public async Task DisposeAsync() => await _context.DisposeAsync();
    #endregion
}