using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

using GoPlay_Core.Business;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services;
using GoPlay_Core.Utils;
using GoPlay_Infra;
using GoPlay_Infra.Repository;

var builder = WebApplication.CreateBuilder(args);


// Configuração de ambiente e settings

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// Configuração da porta no Railway

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Serviços principais

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();

// Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoPlay API", Version = "v1" });

    // Inclui comentários XML no Swagger (se houver)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Banco de Dados

var connectionStringHelper = new ConnectionStringHelper(builder.Configuration);
var connectionString = connectionStringHelper.FromEnvironmentVariable();

builder.Services.AddDbContext<GoPlayDbContext>(options =>
    options.UseNpgsql(connectionString));

// Identity e Autenticação

builder.Services
    .AddIdentity<UserEntity, IdentityRole>()
    .AddEntityFrameworkStores<GoPlayDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Injeção de Dependência (DI)

builder.Services.AddScoped<IUserBusiness<UserEntity>, UserBusiness>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IValidator<UserEntity>, UserEntityValidator>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EmailSender>();

// App Pipeline

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoPlay API v1"));

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseHealthChecks("/health");

app.Run();
