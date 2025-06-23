using System.Net;
using System.Reflection;
using FluentValidation;
using GoPlay_App.Api.Controllers.TournamentManager;
using GoPlay_App.Api.Controllers.UserController.Models;
using GoPlay_Core.Business;
using GoPlay_Core.Business.Interfaces;
using GoPlay_Core.Entities;
using GoPlay_Core.Repository.Interfaces;
using GoPlay_Core.Services;
using GoPlay_Core.Services.Gerencianet;
using GoPlay_Core.Services.Interfaces;
using GoPlay_Core.Validators;
using GoPlay_Infra;
using GoPlay_Infra.Repository;
using GoPlay_Infra.Utils;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region Configuração de Ambiente e Settings

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true) // Para testes locais
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
builder.Services.AddHttpContextAccessor();

#endregion

#region Swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "GoPlay API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
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

builder.Services.AddSingleton<FirebaseNotificationService>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);


#region Injeção de Dependência (DI)

// ---------- Autenticação / Segurança ----------
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailSender>();

// ---------- Domínio: Usuário ----------
builder.Services.AddScoped<IUserBusiness<UserEntity, UserResponse>, UserBusiness>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IValidator<UserEntity>, UserEntityValidator>();

// ---------- Domínio: Torneio ----------
builder.Services.AddScoped<ITournamentBusiness<TournamentEntity>, TournamentBusiness>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<IValidator<TournamentEntity>, TournamentEntityValidator>();

// ---------- Domínio: Categoria ----------
builder.Services.AddScoped<ICategoryBusiness<CategoryEntity>, CategoryBusiness>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IValidator<CategoryEntity>, CategoryEntityValidator>();

// ---------- Domínio: Participantes em Categorias ----------
builder.Services.AddScoped<ICategoryPlayerBusiness, CategoryPlayerBusiness>();
builder.Services.AddScoped<ICategoryPlayerRepository, CategoryPlayerRepository>();

// ---------- Domínio: Partidas e Grupos ----------
builder.Services.AddScoped<IMatchGroupBusiness, MatchGroupBusiness>();
builder.Services.AddScoped<IMatchGroupRepository, MatchGroupRepository>();

// ---------- Domínio: Partidas de Eliminatórias ----------
builder.Services.AddScoped<IGameMatchRepository, GameMatchRepository>();
builder.Services.AddScoped<IGameMatchBusiness, GameMatchBusiness>();

// ---------- Pagamento: Pix / Gerencianet ----------
builder.Services.AddScoped<IPixBusiness, PixBusiness>();
builder.Services.AddScoped<IPixService, PixService>();
builder.Services.AddScoped<GerencianetAuthenticator>();

builder.Services.AddSingleton<CloudinaryService>();
builder.Services.AddScoped<TournamentStatusHandler>();
builder.Services.AddHostedService<TournamentStatusBackgroundService>();

#endregion

#region Pipeline da Aplicação

#region Configuração para Railway / Proxy Reverso

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Aplicação iniciada e log funcionando!");

app.UseForwardedHeaders();

// Middleware opcional para debug de chamadas recebidas
app.Use(async (context, next) =>
{
    Console.WriteLine($"[DEBUG] {context.Request.Method} {context.Request.Path}{context.Request.QueryString}");
    await next();
});

#endregion

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoPlay API v1"));

app.UseRouting();

app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

app.UseHealthChecks("/health");

app.Run();

#endregion
