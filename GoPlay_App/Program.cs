using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using GoPlay_Core.Business;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services;
using GoPlay_Core.Services.Interfaces;
using GoPlay_Core.Utils;
using GoPlay_Infra;
using GoPlay_Infra.Repository;
using GoPlay_Web.Services;

var builder = WebApplication.CreateBuilder(args);

#region Configuração de Ambiente e Settings

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

#endregion

#region Configuração da Porta no Railway

var port = Environment.GetEnvironmentVariable("PORT") ?? "7276";
builder.WebHost.UseUrls($"http://*:{port}");

#endregion

#region Serviços Essenciais

builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();

#endregion

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoPlay API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

#endregion

#region Banco de Dados

var connectionStringHelper = new ConnectionStringHelper(builder.Configuration);
var connectionString = connectionStringHelper.FromEnvironmentVariable();

builder.Services.AddDbContext<GoPlayDbContext>(options =>
    options.UseNpgsql(connectionString));

#endregion

#region Identity e Autenticação

builder.Services
    .AddIdentity<UserEntity, IdentityRole>()
    .AddEntityFrameworkStores<GoPlayDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

#endregion

#region Injeção de Dependência (DI)

builder.Services.AddScoped<IUserBusiness<UserEntity>, UserBusiness>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IValidator<UserEntity>, UserEntityValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddTransient<AccessManagerApi>();
builder.Services.AddTransient<UserManagerApi>();


#endregion


#region Pipeline da Aplicação

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoPlay API v1"));

app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.UseHealthChecks("/health");
app.UseStaticFiles();

app.Run();

#endregion
