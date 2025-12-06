using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace Msd.Services.AuthApi.Extensions;

/// <summary>
/// Extension methods for Swagger/OpenAPI configuration
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger/OpenAPI configuration for JWT Bearer authentication
    /// </summary>
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Auth SSO API",
                Version = "v1",
                Description = "API for Email and Microsoft SSO with ASP.NET Identity and Role-Based authorization",
            });

            option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' followed space and token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            option.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

            // Include XML comments
            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
        });
    }
}
