using CouponService.Abstraction;
using CouponService.Helper.Abstraction;
using CouponService.Helper.CronJobServices;
using CouponService.Helper.CronJobServices.CronJobExtensionMethods;
using CouponService.Helper.Model;
using CouponService.Helper.Repository;
using CouponService.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;

namespace CouponService
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
            services.AddControllers();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddDbContext<Models.DBModels.couponserviceContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            services.AddScoped<ICouponsRepository, CouponsRepository>();
            services.AddScoped<IPlacesRepository, PlacesRepository>();
            services.AddScoped<IPromotionsRepository, PromotionsRepository>();
            services.AddScoped<IIncludedRepository, IncludedRepository>();
            services.AddScoped<IRedemptionRepository, RedemptionRepository>();
            services.AddScoped<ILinksRepository, LinksRepository>();
            services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();

            services.AddCronJob<AnalyticSynced>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                c.CronExpression = @"0 */4 * * *"; // Run every 4 hours
            });

            services.Configure<AzureStorageBlobConfig>(Configuration.GetSection("AzureStorageBlobConfig"));
            services.Configure<Dependencies>(Configuration.GetSection("Dependencies"));
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            var appSettingsSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("MyPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
