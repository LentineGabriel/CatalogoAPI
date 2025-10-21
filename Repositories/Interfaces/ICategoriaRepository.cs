using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Categorias;

namespace CatagoloAPI.Repositories.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<PagedList<Categoria>> GetCategoriesAsync(CategoriasParameters categoriasParams);
    Task<PagedList<Categoria>> GetCategoriesFilteringByNameAsync(CategoriasFiltroNome categoriasFiltroNome);
}