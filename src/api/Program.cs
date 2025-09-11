using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

using Shipstone.AspNetCore.Http;
using Shipstone.Extensions.Identity;
using Shipstone.Extensions.Notifications;
using Shipstone.Extensions.Security;
using Shipstone.Utilities.Security;
using Shipstone.Utilities.Text.Json;

using Shipstone.OpenBook.Api.Core;
using Shipstone.OpenBook.Api.Infrastructure.Authentication;
using Shipstone.OpenBook.Api.Infrastructure.Authorization;
using Shipstone.OpenBook.Api.Infrastructure.Data.EntityFrameworkCore;
using Shipstone.OpenBook.Api.Infrastructure.Data.MySql;
using Shipstone.OpenBook.Api.Infrastructure.Mail;
using Shipstone.OpenBook.Api.Infrastructure.Notifications;
using Shipstone.OpenBook.Api.Web;
using Shipstone.OpenBook.Api.WebApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfigurationSection authenticationSection =
    builder.Configuration.GetRequiredSection("Authentication");

String? connectionString =
    builder.Configuration.GetConnectionString("MySql");

TextWriter ncsaCommonLoggingWriter =
    builder.Environment.IsDevelopment()
        ? new StreamWriter("log.txt", true)
        : TextWriter.Null;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        String? signingKey = authenticationSection["AccessTokenSigningKey"];

        if (signingKey is null)
        {
            throw new InvalidOperationException("The provided configuration does not contain a valid access token signing key.");
        }

        byte[] bytes = Convert.FromBase64String(signingKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(bytes),
            ValidAudience = authenticationSection["Audience"],
            ValidIssuer = authenticationSection["Issuer"],
            ValidateIssuerSigningKey = true
        };
    });

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        JsonConverter converter = new PaginatedListJsonConverterFactory();
        options.JsonSerializerOptions.Converters.Add(converter);
    })
    .AddOpenBookControllers();

builder.Services
    .AddArgumentExceptionHandling()
    .AddIdentityExtensions()
    .AddNcsaCommonLogging(ncsaCommonLoggingWriter)
    .AddNotificationsExtensions()
    .AddPagination(
        builder.Configuration
            .GetSection("Pagination")
            .Bind
    )
    .AddOpenBookCore()
    .AddOpenBookInfrastructureAuthentication(authenticationSection.Bind)
    .AddOpenBookInfrastructureAuthorization()
    .AddOpenBookInfrastructureDataEntityFrameworkCore()
    .AddOpenBookInfrastructureDataMySql(connectionString)
    .AddOpenBookInfrastructureNotifications()
    .AddOpenBookWebAuthorization()
    .AddOpenBookWebClaims()
    .AddOpenBookWebConflictExceptionHandling()
    .AddOpenBookWebForbiddenExceptionHandling()
    .AddOpenBookWebNotFoundExceptionHandling()
    .AddSingleton<IEncryptionService, StubEncryptionService>()
    .AddSingleton<IMailService, StubMailService>()
    .AddSingleton<IPasswordHasher<IPasswordService>, PasswordHasher<IPasswordService>>()
    .AddSingleton<HMAC>(_ =>
    {
        String? key = builder.Configuration["DataProtection:Key"];

        if (key is null)
        {
            throw new NotImplementedException();
        }

        byte[] bytes = Convert.FromBase64String(key);
        return new HMACSHA256(bytes);
    })
    .AddSingleton<RandomNumberGenerator>(_ =>
    {
        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        return new ConcurrentRandomNumberGenerator(rng);
    })
    .AddSingleton<SecurityTokenHandler, JwtSecurityTokenHandler>();

WebApplication app = builder.Build();
app.UseHttpsRedirection();
app.UseNcsaCommonLogging();
app.UseArgumentExceptionHandling();
app.UseOpenBookWebForbiddenExceptionHandling();
app.UseOpenBookWebNotFoundExceptionHandling();
app.UseOpenBookWebConflictExceptionHandling();
app.UseAuthentication();
app.UseOpenBookWebClaims();
app.UseAuthorization();
app.UsePagination();
app.MapControllers();
await app.RunAsync();
return 0;
