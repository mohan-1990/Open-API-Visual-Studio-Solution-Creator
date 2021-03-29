using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Service_Management.Database;
using Service_Management.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Service_Management
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1607
                options.CustomSchemaIds(type => type.ToString());
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Scheme = "bearer"
                });

                // add auth header for [Authorize] endpoints
                options.OperationFilter<AddAuthHeaderOperationFilter>();
            });
            services.AddDbContext<ServiceManagementContext>(options => options.UseSqlServer(Configuration.GetConnectionString("RDB")));
            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration appConfiguration)
        {
            string swaggerUIPrefix = appConfiguration.GetSection("API").GetSection("SwaggerUIPrefix").Value;           

            app.UseSwagger(options =>
            {
                options.RouteTemplate = swaggerUIPrefix + "/{documentName}/swagger.json";
                // Remove the servers in Swagger UI page. This helps the API and the swagger UI pages to be served behind a reverse proxy server
                options.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer>();
                });
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("v1/swagger.json", "Service Management API V1");
                options.RoutePrefix = swaggerUIPrefix;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
