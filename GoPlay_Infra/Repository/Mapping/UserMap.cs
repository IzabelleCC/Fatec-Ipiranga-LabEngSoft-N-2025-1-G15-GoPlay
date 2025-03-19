using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoPlay_Core.Entities;
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

            builder.Property(p => p.Name)
                .HasColumnName("Name")
                .IsRequired();

            builder.Property(p => p.InstagramPage)
                .HasColumnName("InstagramPage");

            builder.Property(p => p.UserType)
                .HasColumnName("UserType")
                .IsRequired();

            builder.Property(p => p.CpfCnpj)
                .HasColumnName("CpfCnpj")
                .IsRequired();

            builder.Property(p => p.Gender)
                .HasColumnName("Gender");

            builder.Property(p => p.BirthDate)
                .HasColumnName("BirthDate");

            builder.Property(p => p.TShirtSize)
                .HasColumnName("TShirtSize");
        }
    }
}
