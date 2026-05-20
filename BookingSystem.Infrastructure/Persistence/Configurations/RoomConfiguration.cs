using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        // Nombre explícito de la tabla
        builder.ToTable("Rooms");

        // Clave primaria
        builder.HasKey(r => r.Id);

        // Propiedades obligatorias
        builder.Property(r => r.Name)
               .IsRequired()
               .HasMaxLength(100); // Nombre corto y manejable

        // Índice único para evitar duplicados a nivel de base de datos
        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Capacity)
               .IsRequired();

        builder.Property(r => r.IsActive)
               .IsRequired();

        // Descripción opcional pero con límite razonable
        builder.Property(r => r.Description)
               .HasMaxLength(500);

        // Índice para búsquedas rápidas por nombre
        // Útil para listados, autocompletado o evitar duplicados a nivel de aplicación
        builder.HasIndex(r => r.Name)
               .HasDatabaseName("IX_Room_Name");
    }
}
