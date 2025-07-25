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

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    // GET
    [HttpGet]
    public ActionResult<IEnumerable<Produto>> Get()
    {
        var produtos = _context.Produtos.AsNoTracking().ToList();

        if(produtos == null) return NotFound();

        return produtos;
    }

    // GET ID
    [HttpGet("{id:int}" , Name = "ObterProduto")]
    public ActionResult<Produto> Get(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

        if(produto == null) return NotFound();

        return produto;
    }

    // POST
    [HttpPost]
    public ActionResult Post(Produto p)
    {
        if(p == null) return BadRequest();

        _context.Produtos.Add(p);
        _context.SaveChanges();

        return new CreatedAtRouteResult("ObterProduto" , new { id = p.ProdutoId } , p);
    }

    // PUT
    [HttpPut("{id:int}")]
    public ActionResult Put(int id , Produto p)
    {
        if(id != p.ProdutoId) return BadRequest();

        _context.Entry(p).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        _context.SaveChanges();

        return Ok(p);
    }

    // DELETE
    [HttpDelete("{id:int}")]
    public ActionResult Delete(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

        if(produto == null) return NotFound("Produto não localizado!");

        _context.Produtos.Remove(produto);
        _context.SaveChanges();

        return Ok(produto);
    }
}
