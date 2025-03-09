using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_UserManagementService_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPlay_UserManagementService_Infra.Repository.Mapping
{
    /// <summary>
    /// Mapeamento da entidade TournamentAdmin
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class TournamentAdminMap : IEntityTypeConfiguration<TournamentAdminEntity>
    {
        /// <summary>
        /// Configuração da entidade TournamentAdmin
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<TournamentAdminEntity> builder)
        {
            builder.ToTable("TOURNAMENT_ADMIN");

            builder.HasKey(t => t.IdUser);

            builder.Property(t => t.IdUser)
                .HasColumnName("ID_USER")
                .IsRequired();

            builder.Property(t => t.Name)
                .HasColumnName("NAME")
                .IsRequired();

            builder.Property(t => t.Email)
                .HasColumnName("EMAIL")
                .IsRequired();

            builder.Property(t => t.Login)
                .HasColumnName("LOGIN")
                .IsRequired();

            builder.Property(t => t.Password)
                .HasColumnName("PASSWORD")
                .IsRequired();

            builder.Property(t => t.InstagramPage)
                .HasColumnName("INSTAGRAM_PAGE");

            builder.Property(t => t.UserTypeId)
                .HasColumnName("USER_TYPE_ID")
                .IsRequired();

            builder.Property(t => t.Cnpj)
                .HasColumnName("CNPJ")
                .IsRequired();
        }
    }
}
