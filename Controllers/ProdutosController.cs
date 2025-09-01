using CatagoloAPI.Context;
using CatagoloAPI.Models;
using CatagoloAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IUnitOfWork uof, ILogger<ProdutosController> logger)
    {
        _uof = uof;
        _logger = logger;
    }

    // GET
    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        _logger.LogInformation("===== Get/Produtos =====");
        var todosProdutos = _uof.ProdutoRepository.GetAll();

        if (todosProdutos == null)
        {
            _logger.LogInformation("===== Erro 404: Produtos não encontrados =====");
            return NotFound("Produtos não encontrados");
        }

        return Ok(todosProdutos);
    }
    
    // GET PRODUTOS C/ CATEGORIA
    [HttpGet("ComCategoria/{id:int:min(1)}")]
    public ActionResult<Produto> GetProdutosCategoria(int id)
    {
        _logger.LogInformation("===== Get/Produtos/ComCategoria =====");
        var produtos = _uof.ProdutoRepository.GetProductsWithCategory(id);

        if (produtos == null)
        {
            _logger.LogInformation("===== Erro 404: Produtos não encontrados. Verificar o ID informada! =====");
            return NotFound("Produtos não encontrados. Por favor, verifique o ID informado!");
        }
        
        return Ok(produtos);
    }

    // GET ID
    [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
    public ActionResult<Produto> GetId(int id)
    {
        _logger.LogInformation($"===== Get/Produtos/id = {id} =====");
        var produtoId = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

        if (produtoId == null)
        {
            _logger.LogInformation($"===== Get/Produtos/id = {id} não encontrado =====");
            return NotFound($"Produto com ID = {id} não encontrado!");
        }

        return Ok(produtoId);
    }

    // POST
    [HttpPost("AdicionarProduto")]
    public ActionResult<Produto> Post(Produto p)
    {
        _logger.LogInformation("===== Post/Produtos/AdicionarProduto =====");
        if (p == null)
        {
            _logger.LogInformation("===== Erro 400: Não foi possível adicionar um novo produto =====");
            return BadRequest("Não foi possível adicionar um novo produto. Tente novamente mais tarde!");
        }

        var produtoCriado = _uof.ProdutoRepository.Create(p);
        _uof.Commit();

        return new CreatedAtRouteResult("ObterProduto" , new { id = produtoCriado.ProdutoId } , produtoCriado);
    }

    // PUT
    [HttpPut("AtualizarProduto/{id:int:min(1)}")]
    public ActionResult<Produto> Put(int id, Produto p)
    {
        _logger.LogInformation($"===== Put/Produtos/AtualizarProduto/id = {id} =====");

        // garantindo que o produto exista antes de atualizar
        if (id != p.ProdutoId)
        {
            _logger.LogInformation("===== Erro 400: O id é diferente do que consta no banco de dados =====");
            return BadRequest("O id é diferente do que consta no banco de dados");
        }

        var produtoExistente = _uof.ProdutoRepository.Update(p);
        _uof.Commit();
        
        return Ok(produtoExistente);
    }

    // DELETE
    [HttpDelete("DeletarProduto/{id:int:min(1)}")]
    public ActionResult<Produto> Delete(int id)
    {
        _logger.LogInformation($"===== Delete/Produtos/id = {id} =====");
        var deletarProduto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);
        
        if (deletarProduto == null)
        {
            _logger.LogInformation($"===== Produto com o id = {id} não encontrada =====");
            return NotFound("Produto não localizado! Verifique o ID digitado.");
        }

        var produtoDeletado = _uof.ProdutoRepository.Delete(deletarProduto);
        _uof.Commit();

        return Ok(produtoDeletado);
    }
}
