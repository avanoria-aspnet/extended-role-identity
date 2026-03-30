using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Identity.Data;

internal class IdentityInitializer()
{
    public static async Task InitilizeDefaultRolesAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        var roles = new List<AppRole>()
        {
            AppRole.Create("Admin", "Use for administrative accounts"),
            AppRole.Create("Member", "Use for standard accounts"),
        };
        
        if (roleManager is not null)
        {
            foreach (var role in roles)
            {
                if (!string.IsNullOrWhiteSpace(role.Name))
                {
                    if (!await roleManager.RoleExistsAsync(role.Name))
                        await roleManager.CreateAsync(role);
                }

            }
        }
    }

    public static async Task InitilizeDefaultAdminAccountsAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

        var users = new List<AppUser>()
        {
            AppUser.Create("admin@domain.com", true),
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
