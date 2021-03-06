using CustomerApi.DatabaseSettings;
using CustomerApi.Middlewares;
using CustomerApi.Models.Request;
using CustomerApi.Repository;
using CustomerApi.Repository.Interfaces;
using CustomerApi.Services;
using CustomerApi.Services.Interfaces;
using CustomerApi.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace CustomerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CustomerDatabaseSettings>(
                Configuration.GetSection(nameof(CustomerDatabaseSettings)));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<CustomerDatabaseSettings>>().Value);
            
            services.AddSingleton<IMongoContext, MongoContext>();
            services.AddSingleton<ICustomerServices, CustomerServices>();
            
            services.AddMvc().AddFluentValidation();
            services.AddTransient<IValidator<CustomerCreateRequestModel>, CustomerCreateValidator>();
            services.AddTransient<IValidator<CustomerUpdateRequestModel>, CustomerUpdateValidator>();
            
            
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "CustomerApi", Version = "v1"});
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerApi v1"));
            }
            
            app.UseMiddleware<ExceptionMiddleware>();
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}