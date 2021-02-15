using System;
using System.Data.Entity.Migrations;
using Kingflix.Domain.DomainModel.IdentityModel;
using Kingflix.Services.Data;
using Kingflix.Services.Data.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Kingflix.Services.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AppDbContext context)
        {

            AppUserManager userManager = new AppUserManager(new UserStore<AppUser>(context), null);
            AppRoleManager roleManager = new AppRoleManager(new RoleStore<AppRole>(context));

            string roleName = "Admin";
            string userName = "Kingflix Admin";
            string password = "KingFlix@123";
            string email = "admin@kingflix.vn";

            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new AppRole(roleName));
            }

            if (!roleManager.RoleExists("Staff"))
            {
                roleManager.Create(new AppRole("Staff"));
            }

            AppUser user = userManager.FindByEmail(email);
            if (user == null)
            {
                AppUser newUser = new AppUser
                {
                    FullName = userName,
                    UserName = email,
                    Email = email,
                    DateCreated = DateTime.Now,
                    EmailConfirmed = true,
                    TimeStep2 = DateTime.Now,
                    PhoneNumber = "0903030303"
                };

                IdentityResult result = userManager.Create(newUser, password);
                user = userManager.Find(email, password);
            }

            if (!userManager.IsInRole(user.Id, roleName))
            {
                userManager.AddToRole(user.Id, roleName);
            }
            context.SaveChanges();
        }
    }
}
