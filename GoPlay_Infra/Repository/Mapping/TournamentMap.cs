using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GoPlay_Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace GoPlay_Infra.Repository.Mapping
{
    [ExcludeFromCodeCoverage]
    public class TournamentMap : IEntityTypeConfiguration<Tournament>
    {
        public void Configure(EntityTypeBuilder<Tournament> builder)
        {
            builder.ToTable("Tournament");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name)
                .HasColumnName("Name")
                .IsRequired();
            builder.Property(t => t.Description)
                .HasColumnName("Description")
                .IsRequired();
            builder.Property(t => t.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();
            builder.Property(t => t.GamesStartDate)
                .HasColumnName("GamesStartDate")
                .IsRequired();
            builder.Property(t => t.GamesEndDate)
                .HasColumnName("GamesEndDate")
                .IsRequired();
            builder.Property(t => t.RegistrationDeadline)
                .HasColumnName("RegistrationDeadline")
                .IsRequired();
            builder.Property(t => t.PaymentDeadline)
                .HasColumnName("PaymentDeadline")
                .IsRequired();
            builder.Property(t => t.Location)
                .HasColumnName("Location")
                .IsRequired();
            builder.Property(t => t.RegistrationFee)
                .HasColumnName("RegistrationFee")
                .IsRequired();
            builder.Property(t => t.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();
            builder.Property(t => t.CourtQuantity)
                .HasColumnName("CourtQuantity")
                .IsRequired();
        }
    }
}
