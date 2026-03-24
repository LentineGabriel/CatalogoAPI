using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using CatagoloAPI.Context;
using CatagoloAPI.Extensions;
using CatagoloAPI.Filters;
using CatagoloAPI.Logging;
using CatagoloAPI.DTO.Mappings;
using CatagoloAPI.Repositories;
using CatagoloAPI.Repositories.Interfaces;
using CatagoloAPI.Models;
using CatagoloAPI.Services.Interfaces;
using CatagoloAPI.Services;

var builder = WebApplication.CreateBuilder(args);

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

    c.AddSecurityDefinition("Bearer" , new OpenApiSecurityScheme()
    {
        Name = "Authorization" ,
        Type = SecuritySchemeType.ApiKey ,
        Scheme = "Bearer" ,
        BearerFormat = "JWT" ,
        In = ParameterLocation.Header ,
        Description = "Insira 'Bearer {seu_token}'."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
#endregion

#region Identity (Blocked Cookies)
builder.Services.AddIdentity<ApplicationUser , IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ISSO AQUI É O QUE RESOLVE O 401
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});
#endregion

#region JWT Authentication
var secretKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new ArgumentException("JWT:SecretKey is missing in configuration.");

var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true ,
        ValidateAudience = true ,
        ValidateLifetime = true ,
        ValidateIssuerSigningKey = true ,
        ClockSkew = TimeSpan.Zero ,
        ValidAudience = builder.Configuration["JWT:ValidAudience"] ,
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"] ,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
#endregion

#region Policies
builder.Services.AddAuthorization(op =>
{
    // all
    op.AddPolicy("AllRoles", policy => policy.RequireRole("Boss", "SuperAdmin", "Admin", "User"));

    // or
    op.AddPolicy("BossOrSuperAdminOrAdmin" , policy => policy.RequireRole("Boss", "SuperAdmin", "Admin"));
    op.AddPolicy("BossOrSuperAdmin", policy => policy.RequireRole("Boss", "SuperAdmin"));

    // only
    op.AddPolicy("BossOnly", policy => policy.RequireRole("Boss"));
    op.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
    op.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    op.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

var origensComAcessoPermitido = "_origensComAcessoPermitido";
builder.Services.AddCors(op =>
{
    op.AddPolicy(name: origensComAcessoPermitido , policy =>
    {
        policy.WithOrigins("https://apirequest.io/");
    });
});
#endregion

#region Database & DI
var mySqlConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnectionString , ServerVersion.AutoDetect(mySqlConnectionString)));

builder.Services.AddScoped(typeof(IRepository<>) , typeof(Repository<>));
builder.Services.AddScoped<ICategoriaRepository , CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository , ProdutoRepository>();
builder.Services.AddScoped<ITokenService , TokenService>();
builder.Services.AddScoped<IUnitOfWork , UnitOfWork>();
builder.Services.AddAutoMapper(cfg => { } , typeof(MappingProfile));

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfig
{
    LogLevel = LogLevel.Information
}));
#endregion

#region App
var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(origensComAcessoPermitido);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
#endregion