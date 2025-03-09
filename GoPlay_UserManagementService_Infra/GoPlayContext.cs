using System;
using System.Diagnostics.CodeAnalysis;
using GoPlay_UserManagementService_Core.Entities;
using GoPlay_UserManagementService_Infra.Repository.Mapping;
using Microsoft.EntityFrameworkCore;


namespace GoPlay_UserManagementService_Infra
{
    [ExcludeFromCodeCoverage]
    public class GoPlayContext : DbContext
    {
        public DbSet<PlayerEntity> PlayerEntity { get; set; }
        public DbSet<TournamentAdminEntity> TournamentAdminEntity { get; set; }

        private string _connectionString = "Host = localhost; Database=FATEC_GOPLAY;Username=postgres;Password=admin;Persist Security Info=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        public GoPlayContext()
        {
        }

        public GoPlayContext(DbContextOptions<GoPlayContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PlayerMap());
            modelBuilder.ApplyConfiguration(new TournamentAdminMap());

            base.OnModelCreating(modelBuilder);
        }

    }
}
