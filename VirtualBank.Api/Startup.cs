using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using VirtualBank.Api.Factories;
using VirtualBank.Api.Services;
using VirtualBank.Core.Entities;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;

namespace VirtualBank.Api
{
    public class Startup
    {
        private readonly string corsPolicy = "AllowAllOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDatabaseContext(Configuration);

            services.AddIdentity()
                    .ConfigureIdentityOptions()
                    .ConfigurePassword();

            services.AddJwtAuthentication(Configuration);

            //Allow all origins
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicy,
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                        builder.AllowAnyHeader();
                        builder.AllowAnyMethod();
                    });
            });

            services.AddOpenAPI();

            services.AddControllers();

            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<ICashTransactionsService, CashTransactionsService>();
            services.AddScoped<ICustomerService, CustomerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.ConfigureOpenApi();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors(corsPolicy);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //serviceProvider.ConfigureDatabseContext().GetAwaiter().GetResult();
        }
    }
}
