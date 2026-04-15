using BookingSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 1. CONFIGURACIÓN DE SERVICIOS
// =========================

// OpenAPI / Swagger
builder.Services.AddOpenApi();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// DbContext con cadena de conexión segura
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("BOOKING_DB_CONNECTION");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Controllers / Minimal APIs
builder.Services.AddControllers();

var app = builder.Build();

// =========================
// 2. PIPELINE HTTP
// =========================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
