using System.Diagnostics.CodeAnalysis;
using GoPlay_Core.Entities;
using GoPlay_Infra.Repository.Mapping;
using GoPlay_UserManagementService_Infra.Repository.Mapping;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoPlay_Infra
{
    [ExcludeFromCodeCoverage]
    public class GoPlayDbContext : IdentityDbContext<UserEntity>
    {
        public GoPlayDbContext(DbContextOptions<GoPlayDbContext> options) : base(options)
        {
        }

        public DbSet<TournamentEntity> Tournaments { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new TournamentMap());
            modelBuilder.ApplyConfiguration(new CategoryMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
