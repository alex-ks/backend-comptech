using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Comptech.Backend.Service.Data;
using Newtonsoft.Json;
using Comptech.Backend.Service.Analytics;

namespace Comptech.Backend.Service
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services

            // for cross-domain AJAX requests
            services.AddCors();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                                                                options.UseNpgsql(connectionString));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext, int>()
                    .AddDefaultTokenProviders();

            services.AddOpenIddict<ApplicationUser, ApplicationRole, ApplicationDbContext, int>()
                    .DisableHttpsRequirement()
                    .SetAccessTokenLifetime(TimeSpan.FromMinutes(15));
            
            services.AddMvc();

            services.AddTransient<SessionValidator>();
            services.AddSingleton<SessionTracker>();
            services.AddSingleton(typeof(IConfiguration), Configuration);
            services.AddSingleton<RecognitionTaskQueue>();

            var analyticsURL = Configuration.GetSection("AnalyticsURL").Value;
            services.AddTransient<IAnalyticsClient, AnalyticsClient>(sp => new AnalyticsClient(analyticsURL));
            var agent = new AnalyticsAgent();
            services.AddSingleton<AnalyticsAgent>();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // for cross-domain AJAX requests
            app.UseCors(builder =>
                   builder.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIdentity();

            app.UseOAuthValidation();
            app.UseOpenIddict();

            app.UseMvc();
        }
    }
}
