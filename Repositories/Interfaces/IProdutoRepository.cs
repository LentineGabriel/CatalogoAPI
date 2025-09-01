using CatagoloAPI.Models;

namespace CatagoloAPI.Repositories.Interfaces;

public interface IProdutoRepository  : IRepository<Produto>
{
    IEnumerable<Produto> GetProductsWithCategory(int id);
}