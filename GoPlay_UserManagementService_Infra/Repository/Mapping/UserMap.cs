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
    /// Mapeamento da entidade Player
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserMap : IEntityTypeConfiguration<UserEntity>
    {
        /// <summary>
        /// Configuração da entidade Player
        /// </summary>
        /// <param name="builder"></param>
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("USER");

            builder.HasKey(p => p.IdUser);

            builder.Property(p => p.IdUser)
                .HasColumnName("ID_USER")
                .IsRequired();

            builder.Property(p => p.Name)
                .HasColumnName("NAME")
                .IsRequired();

            builder.Property(p => p.Email)
                .HasColumnName("EMAIL")
                .IsRequired();

            builder.Property(p => p.Login)
                .HasColumnName("LOGIN")
                .IsRequired();

            builder.Property(p => p.Password)
                .HasColumnName("PASSWORD")
                .IsRequired();

            builder.Property(p => p.InstagramPage)
                .HasColumnName("INSTAGRAMP_PAGE");

            builder.Property(p => p.UserType)
                .HasColumnName("USER_TYPE")
                .IsRequired();

            builder.Property(p => p.Cpf)
                .HasColumnName("CPF");

            builder.Property(p => p.Cnpj)
                .HasColumnName("CNPJ");

            builder.Property(p => p.Gender)
                .HasColumnName("GENDER");

            builder.Property(p => p.BirthDate)
                .HasColumnName("BIRTH_DATE")
                .HasConversion(
                    v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null);

            builder.Property(p => p.TShirtSize)
                .HasColumnName("TSHIRT_SIZE");
        }
    }
}
