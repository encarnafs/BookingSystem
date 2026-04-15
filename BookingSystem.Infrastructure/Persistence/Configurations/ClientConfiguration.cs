using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        // Nombre explícito de la tabla
        builder.ToTable("Clients");

        // Clave primaria
        builder.HasKey(c => c.Id);

        // Nombre completo del cliente
        builder.Property(c => c.FullName)
               .IsRequired()
               .HasMaxLength(150); // Longitud razonable para nombres reales

        // Value Object Email como Owned Type
        builder.OwnsOne(c => c.Email, email =>
        {
            email.Property(e => e.Value)
                 .HasColumnName("Email")
                 .IsRequired()
                 .HasMaxLength(200);

            // Índice único para evitar duplicados de email
            email.HasIndex(e => e.Value)
                 .IsUnique()
                 .HasDatabaseName("IX_Client_Email");
        });

        // Value Object PhoneNumber como Owned Type
        builder.OwnsOne(c => c.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                 .HasColumnName("PhoneNumber")
                 .IsRequired()
                 .HasMaxLength(20); // Suficiente para formatos internacionales

            // Índice para búsquedas rápidas por teléfono
            phone.HasIndex(p => p.Value)
                 .HasDatabaseName("IX_Client_PhoneNumber");
        });
    }
}
