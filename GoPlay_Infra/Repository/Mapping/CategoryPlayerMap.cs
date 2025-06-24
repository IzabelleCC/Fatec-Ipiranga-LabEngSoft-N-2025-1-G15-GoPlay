using GoPlay_Core.Entities;
using GoPlay_Core.Enum;
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

            builder.Property(cp => cp.RegisterStatus)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(cp => cp.FirstUserPaymentConfirmed)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(cp => cp.SecondUserPaymentConfirmed)
                   .IsRequired()
                   .HasDefaultValue(false);

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

            builder.Property(cp => cp.FirstUserTxId)
                        .HasMaxLength(100)
                        .IsRequired(false);

            builder.Property(cp => cp.SecondUserTxId)
                        .HasMaxLength(100)
                        .IsRequired(false);
        }
    }
}
