using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingSystem.Infrastructure.Persistence.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.RoomId).IsRequired();
        builder.Property(b => b.ClientId).IsRequired();
        builder.Property(b => b.CreatedByUserId).IsRequired();
        builder.Property(b => b.Status).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();

        builder.Property(b => b.Comments)
               .HasMaxLength(500);

        // Owned type DateRange
        builder.OwnsOne(b => b.DateRange, dr =>
        {
            dr.Property(d => d.Start)
              .HasColumnName("StartDate")
              .IsRequired();

            dr.Property(d => d.End)
              .HasColumnName("EndDate")
              .IsRequired();

            //dr.HasIndex("StartDate", "EndDate");
        });

        // Índice compuesto usando nombres de columna
        //builder.HasIndex("RoomId", "StartDate", "EndDate");
    }
}
