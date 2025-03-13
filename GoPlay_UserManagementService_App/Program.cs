using FluentValidation;
using GoPlay_UserManagementService_Core.Business;
using GoPlay_UserManagementService_Core.Business.Interfaces;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Core.Repository.Interfaces;
using GoPlay_UserManagementService_Core.Services;
using GoPlay_UserManagementService_Infra;
using GoPlay_UserManagementService_Infra.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserDbContext>((options) => {
    options
        .UseNpgsql(builder.Configuration["ConnectionStrings:GoPlayDb"]);
});

builder.Services
    .AddIdentity<UserEntity, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

// Adicionar serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar seus serviços aqui
builder.Services.AddScoped<IUserBusiness, UserBusiness>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IValidator<UserEntity>, UserEntityValidator>();

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenService>();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();