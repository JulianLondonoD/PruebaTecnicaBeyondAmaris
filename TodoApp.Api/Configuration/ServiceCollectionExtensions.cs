using System.Reflection;
using Microsoft.OpenApi.Models;
using TodoApp.Api.HealthChecks;
using TodoApp.Api.Mappings;

namespace TodoApp.Api.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(ApiMappingProfile));
        
        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("TodoAppCorsPolicy", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        // Swagger
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "TodoApp API",
                Version = "v1",
                Description = "RESTful API for TodoApp with Clean Architecture"
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                c.IncludeXmlComments(xmlPath);
        });

        // Add API-specific health check
        services.AddHealthChecks()
            .AddCheck<BusinessRulesHealthCheck>("business");

        return services;
    }
}
