using CatagoloAPI.Models;
using CatagoloAPI.Pagination.Produtos;
using X.PagedList;

namespace CatagoloAPI.Repositories.Interfaces;

public interface IProdutoRepository  : IRepository<Produto>
{
    Task<IPagedList<Produto>> GetProductsAsync(ProdutosParameters produtosParams);
    Task<IPagedList<Produto>> GetProductsFilteringByPriceAsync(ProdutosFiltroPreco produtosFiltroPreco);
    Task<IEnumerable<Produto>> GetProductsWithCategoryAsync(int id);
}