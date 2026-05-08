using CoreNexus.Domain.Interfaces;
using CoreNexus.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. SOPORTE PARA CONTROLADORES (Indispensable para arquitectura limpia)
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. REGISTRO DE SERVICIOS CORE
builder.Services.AddScoped<ISecureVaultService, SecureVaultService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 3. CONFIGURACIÓN DEL MIDDLEWARE DE AUTENTICACIÓN JWT
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 4. ACTIVACIÓN DE LA SEGURIDAD (El orden importa)
app.UseAuthentication(); // ¿Quién es el usuario?
app.UseAuthorization();  // ¿Qué puede hacer?

app.MapControllers(); // Mapea tu AuthController automáticamente

app.Run();