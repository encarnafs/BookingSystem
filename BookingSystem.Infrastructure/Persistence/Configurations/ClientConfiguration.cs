using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.FullName)
               .IsRequired()
               .HasMaxLength(150);

        // Email VO
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                 .HasColumnName("Email")
                 .IsRequired()
                 .HasMaxLength(200);
        });

        // PhoneNumber VO
        builder.OwnsOne(c => c.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                 .HasColumnName("PhoneNumber")
                 .IsRequired()
                 .HasMaxLength(20);
        });

        // Flags de estado
        builder.Property(c => c.IsActive)
               .IsRequired();

        builder.Property(c => c.IsDeleted)
               .IsRequired();

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

