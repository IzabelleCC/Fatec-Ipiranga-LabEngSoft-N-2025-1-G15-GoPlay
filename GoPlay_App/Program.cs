using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using GoPlay_UserManagementService_Core.Business;
using GoPlay_UserManagementService_Core.Business.Interfaces;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Repository.Interfaces;
using GoPlay_UserManagementService_Core.Services;
using GoPlay_UserManagementService_Infra;
using GoPlay_UserManagementService_Infra.Repository;

var builder = WebApplication.CreateBuilder(args);

// Configura��o do banco de dados
var connectionString = builder.Configuration["ConnectionStrings:GoPlayDb"];
builder.Services.AddDbContext<GoPlayDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Configura��o do Identity
builder.Services
    .AddIdentity<UserEntity, IdentityRole>()
    .AddEntityFrameworkStores<GoPlayDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserClaimsPrincipalFactory<UserEntity>, CustomUserClaimsPrincipalFactory>();

// Configura��o da autoriza��o com roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PlayerPolicy", policy => policy.RequireRole("Player"));
    options.AddPolicy("TournamentAdminPolicy", policy => policy.RequireRole("TournamentAdmin"));
});

// Adicionar servi�os ao cont�iner
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registro de servi�os
builder.Services.AddScoped<IUserBusiness<UserEntity>, UserBusiness>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IValidator<UserEntity>, UserEntityValidator>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();

// Configura��o de autentica��o JWT
var secretKey = builder.Configuration["SymmetricSecurityKey"];
if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("A chave de seguran�a n�o est� configurada.");
}

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateAudience = false,
        ValidateIssuer = false,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Criar roles caso ainda n�o existam
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { "Player", "TournamentAdmin" };
    foreach (var role in roles)
    {
        var roleExists = await roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

// Configura��o do pipeline de requisi��es HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("auth").MapIdentityApi<UserEntity>().WithTags("Authorization");
app.MapControllers();

app.Run();
