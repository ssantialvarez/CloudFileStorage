using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace CloudFileStorage.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CloudFileStorage",
                    Version = "v1",
                    Description = "CloudFileStorage API"
                });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter your JWT token in this field.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT"
                };

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { 
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        []
                    }
                };

                options.AddSecurityRequirement(securityRequirement);
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                options.IncludeXmlComments(xmlPath);
            });
            return services;
        }
    }
}
