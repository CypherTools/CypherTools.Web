using CypherTools.Web.Server.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CypherTools.Web.Server.Data
{
    public static class IdentityDataInitializer
    {
        public static async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }

        public static async Task SeedUsers(UserManager<ApplicationUser> userManager)
        {
            var adminAccount = Environment.GetEnvironmentVariable("CypherToolsWebAdminAccount");
            var adminAccountPassword = Environment.GetEnvironmentVariable("CypherToolsWebAdminPassword");

            if (userManager.FindByNameAsync(adminAccount).Result == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = adminAccount,
                    Email = adminAccount,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                IdentityResult result = await userManager.CreateAsync(user, adminAccountPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrator");
                    await userManager.AddToRoleAsync(user, "GM");
                }
            }
        }

        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("Player").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Player"
                };
                await roleManager.CreateAsync(role);
            }

            if (!roleManager.RoleExistsAsync("GM").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "GM"
                };
                await roleManager.CreateAsync(role);
            }

            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = "Administrator"
                };
                await roleManager.CreateAsync(role);
            }
        }
    }
}
