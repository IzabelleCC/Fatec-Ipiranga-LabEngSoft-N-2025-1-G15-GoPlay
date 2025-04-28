using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace GoPlay_Infra
{
    public class GoPlayDbContextFactory : IDesignTimeDbContextFactory<GoPlayDbContext>
    {
        public GoPlayDbContext CreateDbContext(string[] args)
        {
            // Carrega config direto dos UserSecrets
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets<GoPlayDbContextFactory>()
                .Build();

            var connectionString = configuration.GetConnectionString("GoPlayDb");

            Console.WriteLine($"Connection String: {connectionString}");

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string 'GoPlayDb' not found.");

            var optionsBuilder = new DbContextOptionsBuilder<GoPlayDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new GoPlayDbContext(optionsBuilder.Options);
        }
    }
}
