using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Nombre explícito de la tabla
        builder.ToTable("Users");

        // Clave primaria
        builder.HasKey(u => u.Id);

        // Username: requerido y único
        builder.Property(u => u.Username)
               .IsRequired()
               .HasMaxLength(100);

        builder.HasIndex(u => u.Username)
               .IsUnique()
               .HasDatabaseName("IX_User_Username");

        // PasswordHash: requerido y con longitud fija razonable
        // (normalmente hashes tipo bcrypt/argon2 tienen longitudes conocidas)
        builder.Property(u => u.PasswordHash)
               .IsRequired()
               .HasMaxLength(200);

        // Role: requerido y con longitud limitada
        builder.Property(u => u.Role)
               .IsRequired()
               .HasMaxLength(50);
    }
}
