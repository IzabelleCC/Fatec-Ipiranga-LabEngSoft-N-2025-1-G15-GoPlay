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
    public class GoPlayDbContext : IdentityDbContext<UserEntity>
    {
        public GoPlayDbContext(DbContextOptions<GoPlayDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());
            base.OnModelCreating(modelBuilder);
        }

    }
}
