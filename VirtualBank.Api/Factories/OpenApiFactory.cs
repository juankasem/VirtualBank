using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace VirtualBank.Api.Services
{
    public static class OpenApiFactory
    {

        public static IServiceCollection AddOpenAPI(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Adding swagger document
                options.SwaggerDoc("v1.0", new OpenApiInfo() { Title = "Main Api v1.0", Version = "v1.0" });

                // Include the comments that we wrote in documentation
                //options.IncludeXmlComments("virtualBank.Api.xml");

                // To Use unique names with requests & responses
                options.CustomSchemaIds(x => x.FullName);

                //Define the security schema
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Description = "Jwt Authorization header using the bearer scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference()
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                // Adding the bearer token authentication option to ui
                options.AddSecurityDefinition("Bearer", securityScheme);

                // Use the token provided with the endpoints call
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {securityScheme, new [] { "Bearer"} }
                });

            });

            return services;
        }

        public static IApplicationBuilder ConfigureOpenApi(this IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {

                c.SwaggerEndpoint("/swagger/v1.0/swagger.json", "Versioned API v1.0");

                c.DocExpansion(DocExpansion.None);

            });

            return app;
        }
    }
}
