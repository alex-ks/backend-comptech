using Comptech.Backend.Service.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Comptech.Backend.Service.Test
{
    public class AspApplicationMock
    {
        public UserManager<ApplicationUser> UserManager { get; set; }
        public RoleManager<ApplicationRole> RoleManager { get; set; }
        public SignInManager<ApplicationUser> SignInManager { get; set; }
        public DefaultHttpContext HttpContext { get; set; }
        public IOptions<IdentityOptions> IdentityOptions { get; set; }
        public IServiceProvider ServiceProvider { get; set; }

        public async Task SetUser(ApplicationUser user)
        {
            var factory = new UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>(UserManager,
                                                                                           RoleManager,
                                                                                           IdentityOptions);
            HttpContext.User = await factory.CreateAsync(user);
        }
    }
}
