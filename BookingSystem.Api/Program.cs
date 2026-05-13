using BookingSystem.Api.Authorization;
using BookingSystem.Api.Middleware;
using BookingSystem.Api.Services;
using BookingSystem.Api.Configuration;
using BookingSystem.Application.Bookings.Commands.CreateBooking;
using BookingSystem.Application.Common.Interfaces;
using BookingSystem.Application.DependencyInjection;
using BookingSystem.Infrastructure.Authentication;
using BookingSystem.Infrastructure.DependencyInjection;
using BookingSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
    throw new Exception("JWT Secret debe tener al menos 32 caracteres.");


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
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            // Solo permitir HMAC-SHA256, no otros algoritmos. Buena práctica para evitar ataques  tipo: cambio de algoritmo a none "alg=none", cambio a RS256, usar un algoritmo débil o manipular el header del token. Es una capa extra de seguridad que asegura que el token sólo se valide si fue firmado con el algoritmo esperado.
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
            RequireExpirationTime = true
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

app.MapControllers();

app.Run();
