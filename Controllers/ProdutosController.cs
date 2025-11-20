using AutoMapper;
using CatagoloAPI.Context;
using CatagoloAPI.DTO;
using CatagoloAPI.Models;
using CatagoloAPI.Pagination;
using CatagoloAPI.Pagination.Produtos;
using CatagoloAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

namespace CatagoloAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    #region Props/Ctor
    private readonly IUnitOfWork _uof;
    private readonly ILogger<ProdutosController> _logger;
    private readonly IMapper _mapper;

    public ProdutosController(IUnitOfWork uof, ILogger<ProdutosController> logger, IMapper mapper)
    {
        _uof = uof;
        _logger = logger;
        _mapper = mapper;
    }
    #endregion

    #region GET
    // GET
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetAsync()
    {
        _logger.LogInformation("===== Get/Produtos =====");
        var todosProdutos = await _uof.ProdutoRepository.GetAllAsync();

        if (todosProdutos == null)
        {
            _logger.LogInformation("===== Erro 404: Produtos não encontrados =====");
            return NotFound("Produtos não encontrados");
        }

        var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(todosProdutos);

        return Ok(produtosDTO);
    }

    // GET PAGINAÇÃO
    [HttpGet("Paginacao")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPaginacao([FromQuery] ProdutosParameters produtosParameters)
    {
        var produtos = await _uof.ProdutoRepository.GetProductsAsync(produtosParameters);
        return ObterProdutos(produtos);
    }

    // GET PRODUTOS C/ FILTRO DE PREÇO
    [HttpGet("Filtro/Preco/Paginacao")]
    public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPrecoAsync([FromQuery] ProdutosFiltroPreco produtosFiltroPreco)
    {
        var produtos = await _uof.ProdutoRepository.GetProductsFilteringByPriceAsync(produtosFiltroPreco);
        return ObterProdutos(produtos);
    }

    // GET PRODUTOS C/ CATEGORIA
    [HttpGet("ComCategoria/{id:int:min(1)}")]
    public async Task<ActionResult<ProdutoDTO>> GetProdutosCategoriaAsync(int id)
    {
        _logger.LogInformation("===== Get/Produtos/ComCategoria =====");
        var produtos = await _uof.ProdutoRepository.GetProductsWithCategoryAsync(id);

        if (produtos == null)
        {
            _logger.LogInformation("===== Erro 404: Produtos não encontrados. Verificar o ID informada! =====");
            return NotFound("Produtos não encontrados. Por favor, verifique o ID informado!");
        }
        
        var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        
        return Ok(produtosDTO);
    }

    // GET ID
    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public async Task<ActionResult<ProdutoDTO>> GetId(int id)
    {
        _logger.LogInformation($"===== Get/Produtos/id = {id} =====");
        var produtoId = await _uof.ProdutoRepository.GetIdAsync(p => p.ProdutoId == id);

        if (produtoId == null)
        {
            _logger.LogInformation($"===== Get/Produtos/id = {id} não encontrado =====");
            return NotFound($"Produto com ID = {id} não encontrado!");
        }
        
        var produtoDTO = _mapper.Map<ProdutoDTO>(produtoId);

        return Ok(produtoDTO);
    }
    #endregion

    #region POST
    // POST
    [HttpPost("AdicionarProduto")]
    public async Task<ActionResult<ProdutoDTO>> PostAsync(ProdutoDTO p)
    {
        _logger.LogInformation("===== Post/Produtos/AdicionarProduto =====");
        if (p == null)
        {
            _logger.LogInformation("===== Erro 400: Não foi possível adicionar um novo produto =====");
            return BadRequest("Não foi possível adicionar um novo produto. Tente novamente mais tarde!");
        }

        var produto = _mapper.Map<Produto>(p);

        var produtoCriado = _uof.ProdutoRepository.Create(produto);
        await _uof.CommitAsync();

        var novoProdutoDTO = _mapper.Map<ProdutoDTO>(produtoCriado);

        return new CreatedAtRouteResult("ObterProduto" , new { id = novoProdutoDTO.ProdutoId } , novoProdutoDTO);
    }
    #endregion

    #region PUT
    // PUT
    [HttpPut("AtualizarProduto/{id:int:min(1)}")]
    public async Task<ActionResult<ProdutoDTO>> PutAsync(int id, ProdutoDTO p)
    {
        _logger.LogInformation($"===== Put/Produtos/AtualizarProduto/id = {id} =====");

        // garantindo que o produto exista antes de atualizar
        if (id != p.ProdutoId)
        {
            _logger.LogInformation("===== Erro 400: O id é diferente do que consta no banco de dados =====");
            return BadRequest("O id é diferente do que consta no banco de dados");
        }

        var produto = _mapper.Map<Produto>(p);

        var produtoExistente = _uof.ProdutoRepository.Update(produto);
        await _uof.CommitAsync();
        
        var produtoExistenteDTO = _mapper.Map<ProdutoDTO>(produtoExistente);
        
        return Ok(produtoExistenteDTO);
    }
    #endregion

    #region DELETE
    // DELETE
    [HttpDelete("DeletarProduto/{id:int:min(1)}")]
    public async Task<ActionResult<ProdutoDTO>> DeleteAsync(int id)
    {
        _logger.LogInformation($"===== Delete/Produtos/id = {id} =====");
        var deletarProduto = await _uof.ProdutoRepository.GetIdAsync(p => p.ProdutoId == id);
        
        if (deletarProduto == null)
        {
            _logger.LogInformation($"===== Produto com o id = {id} não encontrada =====");
            return NotFound("Produto não localizado! Verifique o ID digitado.");
        }

        var produtoDeletado = _uof.ProdutoRepository.Delete(deletarProduto);
        await _uof.CommitAsync();
        
        var produtoDeletadoDTO =  _mapper.Map<ProdutoDTO>(produtoDeletado);

        return Ok(produtoDeletadoDTO);
    }
    #endregion

    #region Auxiliary Methods
    private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(IPagedList<Produto> produtos)
    {
        var metadata = new
        {
            produtos.Count ,
            produtos.PageSize ,
            produtos.PageCount ,
            produtos.TotalItemCount ,
            produtos.HasNextPage ,
            produtos.HasPreviousPage
        };
        Response.Headers.Append("X-Pagination" , JsonConvert.SerializeObject(metadata));

        var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
        return Ok(produtosDto);
    }
    #endregion
}
