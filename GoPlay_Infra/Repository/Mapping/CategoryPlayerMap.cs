using GoPlay_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPlay_Infra.Repository.Mapping
{
    public class CategoryPlayerMap : IEntityTypeConfiguration<CategoryPlayerEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryPlayerEntity> builder)
        {
            builder.ToTable("CategoryPlayer");

            builder.HasKey(cp => cp.Id);
            builder.Property(cp => cp.Id).ValueGeneratedOnAdd();

            builder.Property(cp => cp.CategoryId).IsRequired();
            builder.Property(cp => cp.FirstUserId).IsRequired();
            builder.Property(cp => cp.SecondUserId).IsRequired(false);

            builder.HasOne(cp => cp.Category)
                   .WithMany(c => c.CategoryPlayers)
                   .HasForeignKey(cp => cp.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cp => cp.FirstUser)
                   .WithMany()
                   .HasForeignKey(cp => cp.FirstUserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cp => cp.SecondUser)
                   .WithMany()
                   .HasForeignKey(cp => cp.SecondUserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
