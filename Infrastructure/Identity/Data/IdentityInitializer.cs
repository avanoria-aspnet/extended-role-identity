using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.Data;

internal class IdentityInitializer()
{
    public static async Task InitilizeDefaultRolesAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var roles = new List<string>() {  "Admin", "Employee", "Member" };
        
        if (roleManager is not null)
        {
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task InitilizeDefaultAdminAccountsAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var users = new List<AppUser>() 
        { 
            new() { UserName = "admin@domain.com", Email = "admin@domain.com", EmailConfirmed = true }
        };

        if (userManager is not null)
        {
            if (!await userManager.Users.AnyAsync())
            {
                var defaultPassword = "BytMig123!";
                var defaultRoleName = "Admin";

                foreach (var user in users)
                {
                    var created = await userManager.CreateAsync(user, defaultPassword);
                    if (roleManager is not null && created.Succeeded)
                    {
                        if (await roleManager.RoleExistsAsync(defaultRoleName))
                        {
                            await userManager.AddToRoleAsync(user, defaultRoleName);
                        }
                    }
                }

            }
        }
    }
}
