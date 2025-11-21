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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CatagoloAPI.Models;
using CatagoloAPI.Services.Interfaces;

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

var secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new ArgumentException("JWT:SecretKey is missing in configuration.");
builder.Services.AddAuthentication(op =>
{
    op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(op =>
{
    op.SaveToken = true;
    op.RequireHttpsMetadata = false;
    op.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true ,
        ValidateAudience = true ,
        ValidateLifetime = true ,
        ValidateIssuerSigningKey = true ,
        ClockSkew = TimeSpan.Zero ,
        ValidAudience = builder.Configuration["JWT:ValidAudience"] ,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"] ,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});
#endregion

#region Dataase/Instances
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

var mySqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
                                            options.UseMySql(mySqlConnectionString ,
                                            ServerVersion.AutoDetect(mySqlConnectionString)));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
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