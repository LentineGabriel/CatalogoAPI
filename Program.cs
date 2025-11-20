using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CatagoloAPI.Context;
using CatagoloAPI.Extensions;
using CatagoloAPI.Filters;
using CatagoloAPI.Logging;
using System.Text.Json.Serialization;
using CatagoloAPI.DTO.Mappings;
using CatagoloAPI.Repositories;
using CatagoloAPI.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

#region Services
// Add services to the container.

#region Controllers/Swagger
builder.Services.AddControllers(op =>
{
   op.Filters.Add(typeof(ApiExceptionFilter));
}).AddJsonOptions(op =>
{
    op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1" , new OpenApiInfo
    {
        Title = "CatagoloAPI" ,
        Version = "v1" ,
        Description = "API para gerenciamento de categorias e produtos." ,
        Contact = new OpenApiContact
        {
            Name = "Gabriel Lentine" ,
            Email = "gabriellentine66@gmail.com"
        }
    });
});
#endregion

#region Authorization/Authentication
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer").AddJwtBearer();
#endregion

#region Dataase/Instances
var mySqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
                                            options.UseMySql(mySqlConnectionString ,
                                            ServerVersion.AutoDetect(mySqlConnectionString)));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(cfg => {}, typeof(MappingProfile));

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfig
{
    LogLevel = LogLevel.Information
}));
#endregion
#endregion

#region App
var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion