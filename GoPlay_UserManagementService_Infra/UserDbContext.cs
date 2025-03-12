using System;
using System.Diagnostics.CodeAnalysis;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Infra.Repository.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace GoPlay_UserManagementService_Infra
{
    [ExcludeFromCodeCoverage]
    public class UserDbContext : IdentityDbContext<UserEntity>
    {
        
        private string _connectionString = "Host = localhost; Database=FATEC_GOPLAY;Username=postgres;Password=admin;Persist Security Info=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());
            base.OnModelCreating(modelBuilder);
        }

    }
}
