using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace GoPlay_UserManagementService_Infra
{
    [ExcludeFromCodeCoverage]
    public class GoPlayContext : DbContext
    {
        private string _connectionString = "Host = localhost; Database=FATEC_GOPLAY;Username=postgres;Password=admin;Persist Security Info=True";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }


    }
}
