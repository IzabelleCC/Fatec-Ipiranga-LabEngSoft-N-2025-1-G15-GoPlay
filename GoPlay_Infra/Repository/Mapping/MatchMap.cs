using GoPlay_Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoPlay_Infra.Repository.Mapping
{
    public class MatchMap : IEntityTypeConfiguration<MatchEntity>
    {
        public void Configure(EntityTypeBuilder<MatchEntity> builder)
        {
            builder.ToTable("Matches");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ScheduledAt)
                   .IsRequired(false);

            builder.Property(x => x.Result)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.HasOne(x => x.Category)
                   .WithMany()
                   .HasForeignKey(x => x.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Player1)
                   .WithMany()
                   .HasForeignKey(x => x.Player1RegistrationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Player2)
                   .WithMany()
                   .HasForeignKey(x => x.Player2RegistrationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
