using System;
using AutoMapper;
using FluentValidation.AspNetCore;
using Glossary.Api.Filters;
using Glossary.Api.Middlewares;
using Glossary.Core.Abstract.Repositories;
using Glossary.Core.Automapper;
using Glossary.Core.Validators;
using Glossary.Infrastructure;
using Glossary.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Glossary.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment currentEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment = currentEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Glossary API", Version = "v1" });
            });

            services.AddAutoMapper(typeof(AppProfile).Assembly);
            services.AddTransient<IGlossaryRepository, GlossaryRepository>();

            services.AddDbContext<GlossaryDbContext>(
                (options) =>
                {
                    options.UseSqlServer(
                        Configuration.GetConnectionString("DbConnection"),
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                        });
                }, ServiceLifetime.Scoped);

            services
                .AddControllers(options => options.Filters.Add<ValidationFilter>())
                .AddNewtonsoftJson()
                .AddFluentValidation(config =>
                    {
                        config.ImplicitlyValidateChildProperties = false;
                        config.RunDefaultMvcValidationAfterFluentValidationExecutes = true;
                        config.RegisterValidatorsFromAssemblyContaining<GlossaryVmValidator>();
                    })
                .ConfigureApiBehaviorOptions(options =>
                    {
                        options.SuppressModelStateInvalidFilter = true;
                    });

            services.AddCors(c => c.AddPolicy("public", builder =>
             {
                 builder.AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowAnyOrigin();
             }));
        }

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<GlossaryDbContext>())
                {
                    if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                    {
                        context.Database.Migrate();
                    }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // migrate any database changes on startup (includes initial db creation)
            UpdateDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("public");

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Glossary API V1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
