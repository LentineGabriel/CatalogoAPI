using CatagoloAPI.Models;
using CatagoloAPI.Pagination.Categorias;
using X.PagedList;

namespace CatagoloAPI.Repositories.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria>
{
    Task<IPagedList<Categoria>> GetCategoriesAsync(CategoriasParameters categoriasParams);
    Task<IPagedList<Categoria>> GetCategoriesFilteringByNameAsync(CategoriasFiltroNome categoriasFiltroNome);
}