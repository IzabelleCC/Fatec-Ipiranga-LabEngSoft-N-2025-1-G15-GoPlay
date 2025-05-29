using GoPlay_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPlay_Infra.Repository.Mapping
{
    public class CategoryMap : IEntityTypeConfiguration<CategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.ToTable("Category");

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.CategoryType)
                .HasColumnName("CategoryType")
                .IsRequired();

            builder.Property(c => c.PlayerLimit)
                .HasColumnName("PlayerLimit")
                .IsRequired();

            builder.Property(c => c.TournamentId)
                .HasColumnName("TournamentId")
                .IsRequired();

            builder.HasOne<TournamentEntity>()
                .WithMany(t => t.Categories)
                .HasForeignKey(c => c.TournamentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.CategoryPlayers)
                   .WithOne(cp => cp.Category)
                   .HasForeignKey(cp => cp.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
