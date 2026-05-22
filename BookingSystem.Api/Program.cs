using BookingSystem.Api.Authorization;
using BookingSystem.Api.Configuration;
using BookingSystem.Api.Middleware;
using BookingSystem.Api.Services;
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
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
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
// 1. Cargar JwtSettings
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtSettings>();

if (jwtSettings is null || string.IsNullOrWhiteSpace(jwtSettings.Secret))
    throw new InvalidOperationException("JWT Secret no está configurado correctamente.");

if (jwtSettings.Secret.Length < 32)
    throw new InvalidOperationException("JWT Secret debe tener al menos 32 caracteres.");

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// 3. Configurar autenticación JWT y Authorization
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // ============================
        // VALIDACIÓN DEL TOKEN JWT
        // ============================
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
            RequireExpirationTime = true
        };

        // ============================================
        // EVENTOS JWT (VALIDACIÓN + RESPUESTAS 401/403)
        // ============================================
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = context.Principal?.FindFirst(ClaimTypes.Role)?.Value?.Trim();

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                {
                    context.Fail("Token inválido");
                    return;
                }

                if (role == "Client")
                {
                    var clientRepository = context.HttpContext.RequestServices.GetRequiredService<IClientRepository>();
                    var client = await clientRepository.GetByIdAsync(Guid.Parse(userId), context.HttpContext.RequestAborted);

                    if (client == null || !client.IsActive || client.IsDeleted)
                    {
                        context.Fail("Cliente no activo");
                        return;
                    }
                }
                else
                {
                    var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                    var user = await userRepository.GetByIdAsync(Guid.Parse(userId), context.HttpContext.RequestAborted);

                    if (user == null || !user.IsActive || user.IsDeleted)
                    {
                        context.Fail("Usuario no activo");
                        return;
                    }
                }
            },

            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/problem+json";

                var detail = context.Error switch
                {
                    "invalid_token" => "Token inválido o usuario no activo.",
                    "missing_token" => "Falta el token de autenticación.",
                    _ => "Se requiere autenticación para acceder a este recurso."
                };

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Title = "Unauthorized",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = detail
                });
            },

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

// Registrar servicios de autenticación
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
