📘 BookingSystem API

API para la gestión de reservas de salas de reuniones, desarrollada con .NET 9, siguiendo los principios de Clean Architecture, Domain-Driven Design (DDD) y CQRS.

El objetivo del proyecto es construir un backend escalable, mantenible y extensible, aplicando patrones modernos como:

    CQRS + MediatR

    Value Objects

    Repositorios

    Validaciones

    EF Core con Owned Types
    
🚀 Tecnologías

    .NET 9

    C#

    Clean Architecture

    Domain-Driven Design (DDD)

    CQRS + MediatR

    Entity Framework Core 9

    SQL Server

    FluentValidation

    Minimal APIs / Controllers
   

🏛️ Arquitectura del Proyecto

El proyecto sigue la estructura clásica de Clean Architecture:

src/
|-- BookingSystem.Api             (Capa de presentación - endpoints)
|-- BookingSystem.Application     (Casos de uso - Commands, Queries, Handlers)
|-- BookingSystem.Domain          (Entidades, Value Objects, reglas de negocio)
|-- BookingSystem.Infrastructure  (EF Core, repositorios, persistencia)


📦 Capas del Proyecto
✔️ Domain

Contiene la lógica de negocio pura:

    Entidades: Booking, Room, Client, User

    Value Objects: DateRange

    Reglas de negocio:

        Validación de solapamientos

        Confirmación y cancelación de reservas

        Actualización de fechas

        Estados válidos

        Garantía de que Start < End en DateRange

✔️ Application

Implementa los casos de uso mediante CQRS.
Commands

    CreateBooking

    UpdateBookingDates

    UpdateBookingComments

    ConfirmBooking

    CancelBooking

Queries

    GetBookingById

    GetBookingsByRoomId

    GetBookingsByClientId

    GetBookingsInDateRange

Incluye además:

    Validadores con FluentValidation

    Interfaces de repositorios

    Excepciones personalizadas (NotFoundException, ValidationException, etc.)

✔️ Infrastructure

Implementa la persistencia:

    ApplicationDbContext

    Configuración EF Core (incluyendo DateRange como Owned Type)

    Repositorios:

        BookingRepository

        RoomRepository

        ClientRepository

        UserRepository

Migración inicial generada con:

dotnet ef migrations add InitialCreate
dotnet ef database update

✔️ API

Exposición de endpoints REST usando:

    Minimal APIs o Controllers

    MediatR para enviar Commands/Queries

    Manejo global de excepciones

    Documentación con Swagger

📅 Casos de Uso Implementados

Queries

| Tipo  | Caso de uso               |
|-------|----------------------------|
| Query | CheckAvailability          |
| Query | GetAllRooms                |
| Query | GetRoomById                |
| Query | GetAllBookings             |
| Query | GetBookingById             |
| Query | GetBookingsByRoomId        |
| Query | GetBookingsByClientId      |
| Query | GetBookingsInDateRange     |
| Query | GetAllClients              |
| Query | GetClientById              |
| Query | GetAllUsers                |
| Query | GetUserById                |

Commands

| Tipo    | Caso de uso        |
|---------|---------------------|
| Command | LoginClient         |
| Command | LoginUser           |
| Command | RegisterClient      |
| Command | RegisterUser        |
| Command | CreateRoom          |
| Command | UpdateRoom          |
| Command | CreateBooking       |
| Command | UpdateBooking       |
| Command | UpdateBookingDates  |
| Command | UpdateBookingComments |
| Command | ConfirmBooking      |
| Command | CancelBooking       |
| Command | CreateClient        |
| Command | DeleteClient        |
| Command | DisableClient       |
| Command | UpdateClient        |
| Command | CreateUser          |
| Command | DeleteUser          |
| Command | DisableUser         |
| Command | EnableUser          |
| Command | UpdateUser          |
| Command | ChangeUserRole      |

🧠 Reglas de Negocio Principales

    No se pueden crear reservas solapadas en la misma sala

    No se puede modificar una reserva cancelada

    No se puede modificar una reserva finalizada

    Solo se pueden confirmar reservas en estado Pending

    No se puede cancelar una reserva finalizada

    DateRange garantiza que Start < End

🗄️ Persistencia

    EF Core con SQL Server

    DateRange mapeado como Owned Type

    Migración inicial aplicada correctamente

    Repositorios implementados siguiendo las interfaces de Application

🔧 Configuración de la cadena de conexión (User Secrets)

Este proyecto utiliza User Secrets para almacenar la cadena de conexión durante el desarrollo.

📦 Próximos pasos

- Añadir Serilog para logging estructurado

- Completar funcionalidades pendientes en Controllers (Clients, Users)

- Añadir tests de integración (Bookings, Clients, Users, Rooms, Auth)

- Añadir tests unitarios en Application (validadores, handlers)

- Añadir Docker para despliegue

- Añadir CI/CD con GitHub Actions

- Añadir versionado completo de API
    
🤝 Contribuciones

Proyecto en desarrollo activo.
Las contribuciones, sugerencias y mejoras son bienvenidas.
