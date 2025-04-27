using GoPlay_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPlay_Infra.Repository.Mapping
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Category");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.CategoryType)
                .HasColumnName("CategoryType")
                .IsRequired();

            builder.Property(c => c.PlayerLimit)
                .HasColumnName("PlayerLimit")
                .IsRequired();

            builder.Property(c => c.TournamentId)
                .HasColumnName("TournamentId")
                .IsRequired();

            builder.Property(c => c.IsActive)
                .HasColumnName("IsActive")
                .IsRequired();

            builder.HasOne<Tournament>()
                .WithMany(t => t.Categories)
                .HasForeignKey(c => c.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Players)
                .WithMany(u => u.Categories)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoryPlayer",
                    right => right.HasOne<UserEntity>()
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<Category>()
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.HasKey("CategoryId", "UserId");
                        join.Property<int>("CategoryId").HasColumnName("CategoryId");
                        join.Property<int>("UserId").HasColumnName("UserId");
                        join.ToTable("CategoryPlayer");
                    }
                );
        }
    }
}
