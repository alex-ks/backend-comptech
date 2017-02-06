using Comptech.Backend.Service.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Comptech.Backend.Service.Test
{
    public class AspApplicationMockBuilder
    {
        public ServiceCollection Services { get; set; }

        public AspApplicationMockBuilder(IDictionary<string, string> configuration)
        {
            IConfiguration conf = new ConfigurationBuilder()
                .AddInMemoryCollection(configuration)
                .Build();

            Services = new ServiceCollection();
            Services.AddEntityFramework()
                    .AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase());

            Services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext, int>()
                    .AddDefaultTokenProviders();

            Services.AddSingleton(conf);
        }

        public AspApplicationMock Build()
        {
            var app = new AspApplicationMock();

            app.HttpContext = new DefaultHttpContext();

            app.HttpContext.Features.Set<IHttpAuthenticationFeature>(new HttpAuthenticationFeature());
            Services.AddSingleton<IHttpContextAccessor>(h => new HttpContextAccessor
            {
                HttpContext = app.HttpContext
            });

            var serviceProvider = Services.BuildServiceProvider();

            app.UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            app.RoleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            app.SignInManager = serviceProvider.GetRequiredService<SignInManager<ApplicationUser>>();
            app.IdentityOptions = serviceProvider.GetRequiredService<IOptions<IdentityOptions>>();
            app.ServiceProvider = serviceProvider;

            return app;
        }
    }
}
