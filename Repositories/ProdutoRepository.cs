using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Produtos;
using CatagoloAPI.Repositories.Interfaces;
using X.PagedList;

namespace CatagoloAPI.Repositories;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Produto>> GetProductsAsync(ProdutosParameters produtosParams)
    {
        var produtos = await GetAllAsync();
        var produtosOrdenados = produtos.OrderBy(p => p.ProdutoId).AsQueryable();
        var result = await produtosOrdenados.ToPagedListAsync(produtosParams.PageNumber, produtosParams.PageSize);

        return result;
    }

    public async Task<IPagedList<Produto>> GetProductsFilteringByPriceAsync(ProdutosFiltroPreco produtosFiltroPreco)
    {
        var produtos = await GetAllAsync();

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

        var produtosFiltrados = await produtos.ToPagedListAsync(produtosFiltroPreco.PageNumber, produtosFiltroPreco.PageSize);
        return produtosFiltrados;
    }

    public async Task<IEnumerable<Produto>> GetProductsWithCategoryAsync(int id)
    {
        var produtos = await GetAllAsync();
        var produtosComCategoria = produtos.Where(p => p.CategoriaId == id);

        return produtosComCategoria;
    }

    async Task<IPagedList<Produto>> IProdutoRepository.GetProductsAsync(ProdutosParameters produtosParams)
    {
        var produtos = await GetAllAsync();
        var produtosOrdenados = await produtos.ToPagedListAsync(produtosParams.PageNumber, produtosParams.PageSize);

        return produtosOrdenados;
    }
}