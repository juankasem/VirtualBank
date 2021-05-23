using System;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VirtualBank.Api.Factories;
using VirtualBank.Api.Filters;
using VirtualBank.Api.Services;
using VirtualBank.Core.Interfaces;
using VirtualBank.Data;
using VirtualBank.Data.ActionResults;
using VirtualBank.Data.Interfaces;
using VirtualBank.Data.Repositories;

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
            services.AddAutoMapper(typeof(Startup));
            services.AddDatabaseContext(Configuration);

            services.AddIdentity()
                    .ConfigureIdentityOptions()
                    .ConfigurePassword();

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add<ValidationFilter>();
            })
                    .AddFluentValidation(c => c.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddJwtAuthentication(Configuration);

            // Allow all origins
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

            services.AddSingleton<IActionResultProvider, ActionResultProvider>();
            services.AddHttpContextAccessor();

            // Add repositories
            services.AddScoped<IBankAccountRepository, BankAccountRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<IBranchRepository, BranchRepository>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<ICitiesRepository, CitiesRepository>();
            services.AddScoped<ICashTransactionsRepository, CashTransactionsRepository>();
            services.AddScoped<IFastTransactionsRepository, FastTransactionsRepository>();


            // Add services
            services.AddScoped<IBankAccountService, BankAccountService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<ICitiesService, CitiesService>();
            services.AddScoped<ICashTransactionsService, CashTransactionsService>();
            services.AddScoped<ICreditCardsService, CreditCardsService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IDistrictsService, DistrictsService>();
            services.AddScoped<IDebitCardsService, DebitCardsService>();
            services.AddScoped<IFastTransactionsService, FastTransactionsService>();
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
