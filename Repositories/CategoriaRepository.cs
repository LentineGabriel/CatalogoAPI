using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Categorias;
using CatagoloAPI.Repositories.Interfaces;

namespace CatagoloAPI.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedList<Categoria>> GetCategoriesAsync(CategoriasParameters categoriasParams)
    {
        var categorias = await GetAllAsync();
        var categoriasOrdenadas = categorias.OrderBy(c => c.CategoriaId).AsQueryable();
        var result = PagedList<Categoria>.ToPagedList(categoriasOrdenadas, categoriasParams.PageNumber, categoriasParams.PageSize);

        return result;
    }

    public async Task<PagedList<Categoria>> GetCategoriesFilteringByNameAsync(CategoriasFiltroNome categoriasFiltroNome)
    {
        var categoria = await GetAllAsync();
        if(!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
        {
            categoria = categoria.Where(c => c.CategoriaNome!.Contains(categoriasFiltroNome.Nome));
        }
        var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categoria.AsQueryable() , categoriasFiltroNome.PageNumber , categoriasFiltroNome.PageSize);

        return categoriasFiltradas;
    }
}