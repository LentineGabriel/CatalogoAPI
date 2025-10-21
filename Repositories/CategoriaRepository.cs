using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Categorias;
using CatagoloAPI.Repositories.Interfaces;
using X.PagedList;

namespace CatagoloAPI.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IPagedList<Categoria>> GetCategoriesAsync(CategoriasParameters categoriasParams)
    {
        var categorias = await GetAllAsync();
        var categoriasOrdenadas = categorias.OrderBy(c => c.CategoriaId).AsQueryable();
        var result = await categoriasOrdenadas.ToPagedListAsync(categoriasParams.PageNumber, categoriasParams.PageSize);

        return result;
    }

    public async Task<IPagedList<Categoria>> GetCategoriesFilteringByNameAsync(CategoriasFiltroNome categoriasFiltroNome)
    {
        var categoria = await GetAllAsync();
        if(!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
        {
            categoria = categoria.Where(c => c.CategoriaNome!.Contains(categoriasFiltroNome.Nome));
        }
        var categoriasFiltradas = await categoria.ToPagedListAsync(categoriasFiltroNome.PageNumber, categoriasFiltroNome.PageSize);

        return categoriasFiltradas;
    }
}