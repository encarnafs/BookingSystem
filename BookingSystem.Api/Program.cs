using BookingSystem.Api.Authorization;
using BookingSystem.Api.Configuration;
using BookingSystem.Api.Middleware;
using BookingSystem.Api.Services;
using BookingSystem.Application.Bookings.Commands.CreateBooking;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.DependencyInjection;
using BookingSystem.Domain.Entities;
using BookingSystem.Infrastructure.Authentication;
using BookingSystem.Infrastructure.DependencyInjection;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 1. CONFIGURACIÓN DE SERVICIOS
// =========================
//Versionado
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// OpenAPI
//builder.Services.AddOpenApi();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer. Ejemplo: Bearer {token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


// Incluir comentarios XML en Swagger para mejorar la documentación de la API. Esto permite que las descripciones de los controladores, acciones y modelos aparezcan en la interfaz de Swagger UI, lo que facilita a los desarrolladores entender cómo usar la API.
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

//Servicio que permite acceder al HttpContext desde clases que NO son controladores. Sin esto, sólo los controladores tendrían acceso a HttpContext.User, HttpContext.Request, etc. Al registrar IHttpContextAccessor, podemos inyectar ICurrentUserService en cualquier clase (como handlers de MediatR) para obtener información del usuario actual sin acoplar esa clase a ASP.NET Core.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddProblemDetails();

// Registrar servicios de aplicación (handlers, validadores, etc.)
builder.Services.AddApplication();
// Registrar servicios de infraestructura (repositorios, servicios, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

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

if (jwtSettings.Secret.Length < 32)
    throw new InvalidOperationException("JWT Secret debe tener al menos 32 caracteres.");


// 3. Configurar autenticación JWT y Authorization
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Configuración de validación del token JWT
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)),

            // Solo permitir HMAC-SHA256
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],

            RequireExpirationTime = true
        };

        // =========================
        // RESPUESTAS PERSONALIZADAS
        // PARA 401 Y 403
        // =========================
        //
        // Por defecto, JWT devuelve:
        // - 401 Unauthorized
        // - 403 Forbidden
        //
        // con body vacío.
        //
        // Aquí personalizamos esas respuestas para
        // devolver ProblemDetails en formato JSON,
        // manteniendo una API consistente y profesional.
        //
        options.Events = new JwtBearerEvents
        {
            // Se ejecuta cuando el usuario NO está autenticado
            // o el token es inválido/expirado.
            OnChallenge = async context =>
            {
                // Evita que ASP.NET Core escriba
                // la respuesta por defecto.
                context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = "Se requiere autenticación para acceder a este recurso."
            });
        },

        // Se ejecuta cuando el usuario está autenticado
        // pero NO tiene permisos suficientes.
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
                Detail = "No tienes permiso para acceder a este recurso."
            });
        }
     };
     });

// Authorization Policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanCancelBooking", policy =>
        policy.Requirements.Add(new CanCancelBookingRequirement()));

// Registrar el handler de autorización
builder.Services.AddScoped<IAuthorizationHandler, CanCancelBookingHandler>();

// 4. Registrar servicios de autenticación
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordHasher<Client>, PasswordHasher<Client>>();

// Controllers / Minimal APIs
builder.Services.AddControllers();

var app = builder.Build();

// =========================
// 2. PIPELINE HTTP
// =========================

app.UseMiddleware<ExceptionHandlingMiddleware>();

// OpenAPI 
//app.MapOpenApi();

app.UseHttpsRedirection();

app.UseSwagger();
//Esto hace que SwaggerUI muestre v1, v2, v3....
app.UseSwaggerUI(options =>
{
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    foreach (var description in provider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }
});


// Activar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Middleware para manejar errores y devolver respuestas con formato ProblemDetails en caso de códigos de estado 4xx o 5xx. Esto mejora la consistencia de las respuestas de error y facilita el manejo de errores en el cliente.
app.UseStatusCodePages(async context =>
{
    var response = context.HttpContext.Response;

    if (response.HasStarted)
        return;

    response.ContentType = "application/problem+json";

    await response.WriteAsJsonAsync(new ProblemDetails
    {
        Status = response.StatusCode,
        Title = ReasonPhrases.GetReasonPhrase(response.StatusCode)
    });
});

app.MapControllers();

app.Run();
