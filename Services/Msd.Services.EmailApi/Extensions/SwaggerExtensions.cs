using Microsoft.OpenApi.Models;

namespace Msd.Services.EmailApi.Extensions;

/// <summary>
/// Extension methods for Swagger/OpenAPI configuration
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Adds Swagger/OpenAPI configuration
    /// </summary>
    public static void AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Email API",
                Version = "v1",
                Description = "API for sending emails using Azure Communication Services"
            });

            // Include XML comments
            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);
        });
    }
}
