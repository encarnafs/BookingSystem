using BookingSystem.Api.Middleware;
using BookingSystem.Application.Bookings.Commands.CreateBooking;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Infrastructure.Authentication;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BookingSystem.Api.Authorization;
using System.Reflection;
using System.Text;
using BookingSystem.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 1. CONFIGURACIÓN DE SERVICIOS
// =========================

// OpenAPI / Swagger
builder.Services.AddOpenApi();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

//Servicio que permite acceder al HttpContext desde clases que NO son controladores. Sin esto, sólo los controladores tendrían acceso a HttpContext.User, HttpContext.Request, etc. Al registrar IHttpContextAccessor, podemos inyectar ICurrentUserService en cualquier clase (como handlers de MediatR) para obtener información del usuario actual sin acoplar esa clase a ASP.NET Core.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddProblemDetails();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateBookingCommand).Assembly));

// DbContext con cadena de conexión segura
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("BOOKING_DB_CONNECTION");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// =========================
// . CONFIGURACIÓN JWT
// =========================
// 1. Cargar JwtSettings (Issuer, Audience, ExpiryMinutes + Secret desde User Secrets)
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);

// 2. Registrar JwtSettings para inyección
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// 3. Configurar autenticación JWT y Authorization
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

// Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanCancelBooking", policy =>
        policy.Requirements.Add(new CanCancelBookingRequirement()));
});
// Registrar el handler de autorización
builder.Services.AddScoped<IAuthorizationHandler, CanCancelBookingHandler>();

// 4. Registrar servicios de autenticación
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();


// Controllers / Minimal APIs
builder.Services.AddControllers();

var app = builder.Build();

// =========================
// 2. PIPELINE HTTP
// =========================

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Activar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
