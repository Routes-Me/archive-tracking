using System;
using ArchiveTrackService.Abstraction;
using ArchiveTrackService.Helper.Abstraction;
using ArchiveTrackService.Helper.CronJobServices;
using ArchiveTrackService.Helper.CronJobServices.CronJobExtensionMethods;
using ArchiveTrackService.Helper.Repository;
using ArchiveTrackService.Models;
using ArchiveTrackService.Models.DBModels;
using ArchiveTrackService.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace ArchiveTrackService
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

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddCronJob<RemoveSynced>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Local;
                //c.CronExpression = @"*/1 * * * * *";
                c.CronExpression = @"0 3 1 */2 *"; //  Run every 60 days at 3 AM
            });

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();

            services.AddDbContext<archivetrackserviceContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });

            //Registered services
            services.AddScoped<ICoordinateRepository, CoordinateRepository>();
            services.AddScoped<IFeedsIncludedRepository, FeedsIncludedRepository>();

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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
