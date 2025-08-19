using CatagoloAPI.Context;
using CatagoloAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(AppDbContext context , ILogger<ProdutosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Produto>>> Get()
    {
        try
        {
            _logger.LogInformation("===== Get/Produtos =====");
            var produtos = await _context.Produtos.AsNoTracking().ToListAsync();
            if(produtos == null) return NotFound();

            return produtos;
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    // GET ID
    [HttpGet("{id:int:min(1)}" , Name = "ObterProduto")]
    public async Task<ActionResult<Produto>> Get(int id)
    {
        try
        {
            _logger.LogInformation($"===== Get/Produtos/id = {id} =====");
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.ProdutoId == id);
            if(produto == null) return NotFound();

            return produto;
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    // POST
    [HttpPost("AdicionarProduto")]
    public async Task<ActionResult> Post(Produto p)
    {
        try
        {
            _logger.LogInformation("===== Post/Produtos/AdicionarProduto =====");
            if(p == null) return BadRequest();

            await _context.Produtos.AddAsync(p);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("ObterProduto" , new { id = p.ProdutoId } , p);
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    // PUT
    [HttpPut("AtualizarProduto/{id:int:min(1)}")]
    public async Task<ActionResult> Put(int id , Produto p)
    {
        try
        {
            _logger.LogInformation($"===== Put/Produtos/AtualizarProduto/id = {id} =====");

            // garantindo que o produto exista antes de atualizar
            if(id != p.ProdutoId) return BadRequest();

            var produtoExistente = await _context.Produtos.FindAsync(id);
            if(produtoExistente == null) return NotFound("Produto não localizado!");

            produtoExistente.ProdutoNome = p.ProdutoNome;
            produtoExistente.ProdutoDescricao = p.ProdutoDescricao;
            produtoExistente.ProdutoPreco = p.ProdutoPreco;

            await _context.SaveChangesAsync();
            return Ok(produtoExistente);
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }

    // DELETE
    [HttpDelete("DeletarProduto/{id:int:min(1)}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            _logger.LogInformation($"===== Delete/Produtos/id = {id} =====");
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.ProdutoId == id);
            if(produto == null) return NotFound("Produto não localizado!");

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return Ok(produto);
        }
        catch(Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError , "Ocorreu um problema ao tratar a sua solicitação.");
        }
    }
}
