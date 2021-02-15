using System;
using System.Web.Mvc;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Services.Data.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

namespace Kingflix.Website.App_Start
{
    public partial class AutofacConfig
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            //app.CreatePerOwinContext(() => DependencyResolver.Current.GetService<AppUserManager>());
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, AppUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseFacebookAuthentication(
               appId: "977819312687491",
               appSecret: "9af3b1101c4e3eca017b45afa19bd5f0");
            app.UseGoogleAuthentication(
               clientId: "610851523183-osh0vfe1hqm0nl955dpj0t5lqjvmnfth.apps.googleusercontent.com",
               clientSecret: "-M8oLEQPgRwiEMGX1jrK1HXk"
            );
        }
    }
}