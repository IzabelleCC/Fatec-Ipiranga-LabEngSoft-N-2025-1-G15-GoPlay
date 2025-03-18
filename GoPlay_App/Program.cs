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
using GoPlay_Core.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do banco de dados
var connectionString = builder.Configuration["ConnectionStrings:GoPlayDb"];
builder.Services.AddDbContext<GoPlayDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services
        .AddIdentityApiEndpoints<UserEntity>()
        .AddEntityFrameworkStores<GoPlayDbContext>();

// Registro de serviços
builder.Services.AddScoped<IUserBusiness<UserEntity>, UserBusiness>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IValidator<UserEntity>, UserEntityValidator>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EmailSender>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapSwagger();


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("auth").MapIdentityApi<UserEntity>().WithTags("Authentication");
app.MapControllers();

app.Run();
