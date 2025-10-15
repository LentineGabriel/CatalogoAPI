using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Produtos;
using CatagoloAPI.Repositories.Interfaces;

namespace CatagoloAPI.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<Produto> GetProducts(ProdutosParameters produtosParams)
    {
        return GetAll()
            .OrderBy(p => p.ProdutoId)
            .Skip((produtosParams.PageNumber - 1) * produtosParams.PageSize)
            .Take(produtosParams.PageSize)
            .ToList();
    }

    public PagedList<Produto> GetProductsFilteringByPrice(ProdutosFiltroPreco produtosFiltroPreco)
    {
        var produtos = GetAll().AsQueryable();

        if(produtosFiltroPreco.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroPreco.PrecoCriterio))
        {
            switch(produtosFiltroPreco.PrecoCriterio.ToLower())
            {
                case "maior":
                produtos = produtos.Where(p => p.ProdutoPreco >= produtosFiltroPreco.Preco.Value);
                break;
                case "menor":
                produtos = produtos.Where(p => p.ProdutoPreco <= produtosFiltroPreco.Preco.Value);
                break;
                case "igual":
                produtos = produtos.Where(p => p.ProdutoPreco == produtosFiltroPreco.Preco.Value);
                break;
                default:
                throw new ArgumentException("Criterio de preço inválido");
            }
        }

        var produtosFiltrados = PagedList<Produto>.ToPagedList(produtos, produtosFiltroPreco.PageNumber, produtosFiltroPreco.PageSize);
        return produtosFiltrados;
    }

    public IEnumerable<Produto> GetProductsWithCategory(int id) => GetAll().Where(p => p.CategoriaId == id);

    PagedList<Produto> IProdutoRepository.GetProducts(ProdutosParameters produtosParams)
    {
        var produtos = GetAll().OrderBy(p => p.ProdutoId).AsQueryable();
        var produtosOrdenados = PagedList<Produto>.ToPagedList(produtos , produtosParams.PageNumber , produtosParams.PageSize);

        return produtosOrdenados;
    }
}