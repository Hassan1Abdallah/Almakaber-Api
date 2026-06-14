using Almakaber.DAL.Entities;
using Microsoft.AspNetCore.Identity;

namespace Almakaber.API.Extensions
{
    public static class IdentitySeed
    {
        public static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            var adminEmail = "admin@almakaber.com";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    FullName = "Admin System",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    OtpCode = null,
                    OtpExpiryTime = null
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@23456");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}