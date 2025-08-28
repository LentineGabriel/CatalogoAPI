using CatagoloAPI.Models;

namespace CatagoloAPI.Repositories.Interfaces;

public interface IProdutoRepository
{
    IEnumerable<Produto> GetAll();
    Produto GetById(int id);
    Produto Create(Produto produto);
    Produto  Update(Produto produto);
    Produto  Delete(int id);
}