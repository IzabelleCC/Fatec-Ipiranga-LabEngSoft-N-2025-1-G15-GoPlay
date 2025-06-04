using GoPlay_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoPlay_Infra.Repository.Mapping
{
    public class MatchGroupMap : IEntityTypeConfiguration<MatchGroupEntity>
    {
        public void Configure(EntityTypeBuilder<MatchGroupEntity> builder)
        {
            builder.ToTable("MatchGroup");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedOnAdd();

            builder.Property(m => m.CategoryId)
                .IsRequired();

            builder.Property(m => m.GroupNumber)
                .IsRequired();

            builder.Property(m => m.RegistrationCategoryId)
                .IsRequired();

            builder.Property(m => m.ScheduledAt)
                .IsRequired(false);

            builder.Property(m => m.Result)
                .HasMaxLength(255)
                .IsRequired(false);

            builder.Property(m => m.AttendanceConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            // Relacionamento com Category (Category.MatchGroups)
            builder.HasOne(m => m.Category)
                .WithMany(c => c.MatchGroups)
                .HasForeignKey(m => m.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com CategoryPlayer (CategoryPlayer.MatchGroups)
            builder.HasOne(m => m.RegistrationCategory)
                .WithMany(cp => cp.MatchGroups)
                .HasForeignKey(m => m.RegistrationCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
