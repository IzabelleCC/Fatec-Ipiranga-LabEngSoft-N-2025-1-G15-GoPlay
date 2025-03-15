using System;
using System.Diagnostics.CodeAnalysis;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Infra.Repository.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


namespace GoPlay_UserManagementService_Infra
{
    [ExcludeFromCodeCoverage]
    public class UserDbContext : IdentityDbContext<UserEntity>
    {
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
