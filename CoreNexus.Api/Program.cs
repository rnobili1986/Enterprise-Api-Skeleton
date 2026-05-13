using CoreNexus.Application.Model;
using CoreNexus.Domain.Interfaces;
using CoreNexus.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Serilog;
using CoreNexus.Api.Middleware;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// 1. CONFIGURACIÓN DE OPCIONES (Pattern Options)
// Esto debe ir antes de registrar los servicios que lo usan
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

// 2. SOPORTE PARA CONTROLADORES
builder.Services.AddControllers();

// 3. CONFIGURACIÓN DE SWAGGER CON JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CoreNexus API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa 'Bearer' [espacio] y luego tu token.\n\nEjemplo: \"Bearer eyJhbGciOiJIUzI1...\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// 4. REGISTRO DE SERVICIOS CORE (DI)
builder.Services.AddScoped<ISecureVaultService, SecureVaultService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 5. CONFIGURACIÓN DEL MIDDLEWARE DE AUTENTICACIÓN JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

builder.Services.AddAuthentication(options =>
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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

var app = builder.Build();

// 6. PIPELINE DE LA APLICACIÓN
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseHttpsRedirection();

// EL ORDEN ES CRÍTICO AQUÍ:
app.UseAuthentication(); // 1. Primero autentica (¿Quién eres?)
app.UseAuthorization();  // 2. Luego autoriza (¿Tienes permiso?)

app.MapControllers();

app.Run();