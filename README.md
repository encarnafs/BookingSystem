📘 BookingSystem API

API para la gestión de reservas de salas de reuniones, desarrollada con .NET 9, siguiendo los principios de Clean Architecture y Domain-Driven Design (DDD).

Este proyecto está diseñado para ser escalable, mantenible y fácil de extender, aplicando patrones modernos como CQRS, MediatR, Value Objects y Repositorios.
🚀 Tecnologías

    .NET 9

    C#

    Clean Architecture

    Domain-Driven Design (DDD)

    CQRS + MediatR

    Entity Framework Core

    SQL Server

    FluentValidation

🏛️ Arquitectura

El proyecto sigue la estructura clásica de Clean Architecture:
Código

src/
 ├── BookingSystem.Api           → Capa de presentación (endpoints)
 ├── BookingSystem.Application   → Casos de uso (Commands, Queries, Handlers)
 ├── BookingSystem.Domain        → Entidades, Value Objects, reglas de negocio
 └── BookingSystem.Infrastructure→ EF Core, repositorios, persistencia

✔️ Domain

Contiene la lógica de negocio pura:

    Entidades: Booking, Room, Client, User

    Value Objects: DateRange

    Reglas de negocio (métodos como Confirm(), Cancel(), UpdateDates())

✔️ Application

Contiene los casos de uso:

    Commands:

        CreateBooking

        UpdateBookingDates

        UpdateBookingComments

        ConfirmBooking

        CancelBooking

    Queries:

        GetBookingById

        GetBookingsByRoomId

        GetBookingsByClientId

        GetBookingsInDateRange

Incluye también:

    Validadores con FluentValidation

    Interfaces de repositorios

    Excepciones personalizadas

✔️ Infrastructure

Implementa:

    ApplicationDbContext

    Configuración EF Core (incluyendo DateRange como Owned Type)

    Repositorios:

        BookingRepository

        RoomRepository

        ClientRepository

        UserRepository

✔️ API

Expondrá los endpoints REST usando:

    Minimal APIs o Controllers (según prefieras)

    MediatR para enviar Commands/Queries

    Manejo global de excepciones

📅 Casos de uso implementados
🔹 Bookings
Tipo	Caso de uso
Query	GetBookingById
Query	GetBookingsByRoomId
Query	GetBookingsByClientId
Query	GetBookingsInDateRange
Command	CreateBooking
Command	UpdateBookingDates
Command	UpdateBookingComments
Command	ConfirmBooking
Command	CancelBooking
🧠 Reglas de negocio principales

    No se pueden crear reservas solapadas en la misma sala

    No se puede modificar una reserva cancelada

    No se puede modificar una reserva ya finalizada

    Solo se pueden confirmar reservas en estado Pending

    No se puede cancelar una reserva ya finalizada

    DateRange garantiza que Start < End

🗄️ Persistencia

    EF Core con SQL Server

    DateRange mapeado como Owned Type

    Repositorios implementados siguiendo interfaces de Application

📦 Próximos pasos

    Implementar los repositorios en Infrastructure

    Crear los endpoints en la API

    Añadir autenticación con JWT

    Añadir Docker para despliegue

    Añadir tests de integración

🤝 Contribuciones

Este proyecto está en desarrollo activo.
Las contribuciones, sugerencias y mejoras son bienvenidas
