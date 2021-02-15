using System.Security.Claims;
using System.Threading.Tasks;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Services.Data.Identity.Abstraction;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Kingflix.Services.Data.Identity
{
    public class AppSignInManager : SignInManager<AppUser, string>
    {

        public AppSignInManager(AppUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {

        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(AppUser user)
        {
            return user.GenerateUserIdentityAsync((AppUserManager)UserManager);
        }
    }
}
