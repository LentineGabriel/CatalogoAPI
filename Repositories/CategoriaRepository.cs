using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Categorias;
using CatagoloAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Repositories;

public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
{
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

    public PagedList<Categoria> GetCategories(CategoriasParameters categoriasParams)
    {
        var categorias = GetAll().OrderBy(c => c.CategoriaId).AsQueryable();
        var categoriasOrdenadas = PagedList<Categoria>.ToPagedList(categorias, categoriasParams.PageNumber, categoriasParams.PageSize);

        return categoriasOrdenadas;
    }

    public PagedList<Categoria> GetCategoriesFilteringByName(CategoriasFiltroNome categoriasFiltroNome)
    {
        var categoria = GetAll().AsQueryable();
        if(!string.IsNullOrEmpty(categoriasFiltroNome.Nome))
        {
            categoria = categoria.Where(c => c.CategoriaNome.Contains(categoriasFiltroNome.Nome));
        }
        var categoriasFiltradas = PagedList<Categoria>.ToPagedList(categoria , categoriasFiltroNome.PageNumber , categoriasFiltroNome.PageSize);

        return categoriasFiltradas;
    }
}