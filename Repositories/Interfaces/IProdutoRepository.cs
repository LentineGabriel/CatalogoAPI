using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Produtos;

namespace CatagoloAPI.Repositories.Interfaces;

public interface IProdutoRepository  : IRepository<Produto>
{
    Task<PagedList<Produto>> GetProductsAsync(ProdutosParameters produtosParams);
    Task<PagedList<Produto>> GetProductsFilteringByPriceAsync(ProdutosFiltroPreco produtosFiltroPreco);
    Task<IEnumerable<Produto>> GetProductsWithCategoryAsync(int id);
}