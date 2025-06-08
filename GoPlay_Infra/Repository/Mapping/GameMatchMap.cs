using GoPlay_Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace GoPlay_Infra.Repository.Mapping
{
    public class GameMatchMap : IEntityTypeConfiguration<GameMatchEntity>
    {
        public void Configure(EntityTypeBuilder<GameMatchEntity> builder)
        {
            builder.ToTable("GameMatch");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Competitor1Id).IsRequired();

            builder.Property(g => g.Competitor2Id);

            builder.Property(g => g.MatchStage).IsRequired();

            builder.Property(g => g.MatchTime);

            builder.Property(g => g.CourtNumber);

            builder.Property(g => g.QtdGames1);

            builder.Property(g => g.QtdGames2);

            builder.Property(g => g.Result);

            builder.Property(g => g.NumberGame)
                .IsRequired()
                .HasDefaultValue(0);

            builder.HasOne(g => g.Competitor1)
                .WithMany()
                .HasForeignKey(g => g.Competitor1Id)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.Competitor2)
                .WithMany()
                .HasForeignKey(g => g.Competitor2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

