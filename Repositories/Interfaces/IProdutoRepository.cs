using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Produtos;

namespace CatagoloAPI.Repositories.Interfaces;

public interface IProdutoRepository  : IRepository<Produto>
{
    PagedList<Produto> GetProducts(ProdutosParameters produtosParams);
    PagedList<Produto> GetProductsFilteringByPrice(ProdutosFiltroPreco produtosFiltroPreco);
    IEnumerable<Produto> GetProductsWithCategory(int id);
}