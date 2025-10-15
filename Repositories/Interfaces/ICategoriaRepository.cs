using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Categorias;

namespace CatagoloAPI.Repositories.Interfaces;

public interface ICategoriaRepository : IRepository<Categoria>
{
    PagedList<Categoria> GetCategories(CategoriasParameters categoriasParams);
    PagedList<Categoria> GetCategoriesFilteringByName(CategoriasFiltroNome categoriasFiltroNome);
}