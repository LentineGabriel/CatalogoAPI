﻿using CatagoloAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatagoloAPI.Context;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
}
