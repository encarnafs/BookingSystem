using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        // Nombre explícito de la tabla
        builder.ToTable("Bookings");

        // Clave primaria
        builder.HasKey(b => b.Id);

        // Propiedades obligatorias
        builder.Property(b => b.RoomId).IsRequired();
        builder.Property(b => b.ClientId).IsRequired();
        builder.Property(b => b.CreatedByUserId).IsRequired();
        builder.Property(b => b.Status).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();

        // Comentarios con longitud limitada
        builder.Property(b => b.Comments)
               .HasMaxLength(500);

        // Value Object DateRange como Owned Type
        builder.OwnsOne(b => b.DateRange, dr =>
        {
            dr.Property(p => p.Start)
              .HasColumnName("StartDate")
              .IsRequired();

            dr.Property(p => p.End)
              .HasColumnName("EndDate")
              .IsRequired();
        });

        // Índice compuesto para optimizar consultas de disponibilidad
        // Este índice NO evita solapamientos (eso lo hace el dominio),
        // pero acelera las búsquedas de reservas por habitación y rango de fechas.
        builder.HasIndex("RoomId", "StartDate", "EndDate");
    }
}

