using CatagoloAPI.Models;

namespace CatagoloAPI.Repositories.Interfaces;

public interface ICategoriaRepository
{
    IEnumerable<Categoria> GetAll();
    IEnumerable<Categoria> GetAllWithProducts();
    Categoria GetById(int id);
    Categoria Create(Categoria categoria);
    Categoria  Update(Categoria categoria);
    Categoria  Delete(int id);
}