using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Domain.Abstractions; // ⭐ Necesario para IHasDomainEvents
using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingSystem.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventDispatcher domainEventDispatcher)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher; 
    }

    // DbSets: representan las tablas principales del sistema
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<User> Users => Set<User>();

    // Configuración del modelo
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Aplica automáticamente TODAS las configuraciones de IEntityTypeConfiguration<T>
        // dentro del ensamblado de Infrastructure.
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Guardamos primero los cambios en la base de datos
        var result = await base.SaveChangesAsync(cancellationToken);

        // 2. Recogemos las entidades que tienen Domain Events
        var domainEntities = ChangeTracker
            .Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Any())
            .ToList();

        // 3. Extraemos todos los Domain Events
        var domainEvents = domainEntities
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();

        // 4. Limpiamos los Domain Events de las entidades
        domainEntities.ForEach(e => e.Entity.ClearDomainEvents());

        // 5. Publicamos los eventos usando Application
        foreach (var domainEvent in domainEvents)
        {
            await _domainEventDispatcher.Dispatch(domainEvent);
        }

        return result;
    }
}
