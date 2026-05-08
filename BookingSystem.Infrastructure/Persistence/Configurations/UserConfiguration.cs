using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(u => u.Username)
               .IsUnique()
               .HasDatabaseName("IX_User_Username");

        // Email VO (ESTO ES LO QUE FALTABA)
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                 .HasColumnName("Email")
                 .IsRequired()
                 .HasMaxLength(200);
        });

        builder.Property(u => u.PasswordHash)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(u => u.Role)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.IsActive)
               .IsRequired();

        builder.Property(u => u.IsDeleted)
               .IsRequired();

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}

