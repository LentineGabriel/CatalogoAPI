using AutoMapper;
using CatagoloAPI.Context;
using CatagoloAPI.DTO;
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
    private readonly IMapper _mapper;

    public ProdutosController(IUnitOfWork uof, ILogger<ProdutosController> logger, IMapper mapper)
    {
        _uof = uof;
        _logger = logger;
        _mapper = mapper;
    }

    // GET
    [HttpGet]
    public ActionResult<IEnumerable<ProdutoDTO>> Get()
    {
        _logger.LogInformation("===== Get/Produtos =====");
        var todosProdutos = _uof.ProdutoRepository.GetAll();

        if (todosProdutos == null)
        {
            _logger.LogInformation("===== Erro 404: Produtos não encontrados =====");
            return NotFound("Produtos não encontrados");
        }

        var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(todosProdutos);
        
        return Ok(produtosDTO);
    }
    
    // GET PRODUTOS C/ CATEGORIA
    [HttpGet("ComCategoria/{id:int:min(1)}")]
    public ActionResult<ProdutoDTO> GetProdutosCategoria(int id)
    {
        _logger.LogInformation("===== Get/Produtos/ComCategoria =====");
        var produtos = _uof.ProdutoRepository.GetProductsWithCategory(id);

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
    public ActionResult<ProdutoDTO> GetId(int id)
    {
        _logger.LogInformation($"===== Get/Produtos/id = {id} =====");
        var produtoId = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

        if (produtoId == null)
        {
            _logger.LogInformation($"===== Get/Produtos/id = {id} não encontrado =====");
            return NotFound($"Produto com ID = {id} não encontrado!");
        }
        
        var produtoDTO = _mapper.Map<ProdutoDTO>(produtoId);

        return Ok(produtoDTO);
    }

    // POST
    [HttpPost("AdicionarProduto")]
    public ActionResult<ProdutoDTO> Post(ProdutoDTO p)
    {
        _logger.LogInformation("===== Post/Produtos/AdicionarProduto =====");
        if (p == null)
        {
            _logger.LogInformation("===== Erro 400: Não foi possível adicionar um novo produto =====");
            return BadRequest("Não foi possível adicionar um novo produto. Tente novamente mais tarde!");
        }

        var produto = _mapper.Map<Produto>(p);

        var produtoCriado = _uof.ProdutoRepository.Create(produto);
        _uof.Commit();

        var novoProdutoDTO = _mapper.Map<ProdutoDTO>(produtoCriado);

        return new CreatedAtRouteResult("ObterProduto" , new { id = novoProdutoDTO.ProdutoId } , novoProdutoDTO);
    }

    // PUT
    [HttpPut("AtualizarProduto/{id:int:min(1)}")]
    public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO p)
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
        _uof.Commit();
        
        var produtoExistenteDTO = _mapper.Map<ProdutoDTO>(produtoExistente);
        
        return Ok(produtoExistenteDTO);
    }

    // DELETE
    [HttpDelete("DeletarProduto/{id:int:min(1)}")]
    public ActionResult<ProdutoDTO> Delete(int id)
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
        
        var produtoDeletadoDTO =  _mapper.Map<ProdutoDTO>(produtoDeletado);

        return Ok(produtoDeletadoDTO);
    }
}
