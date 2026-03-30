using Application.Abstractions.Authentication;
using Application.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Services;

public class IdentityAuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager) : IAuthService
{
    public async Task<AuthResult> SignUpUserAsync(string email, string password, string roleName = "Member")
    {
        if (string.IsNullOrWhiteSpace(email))
            return new AuthResult(false, "Email address must be provided");

        if (string.IsNullOrWhiteSpace(password))
            return new AuthResult(false, "Password must be provided");

        var user = await userManager.FindByEmailAsync(email);
        if (user is not null)
            return new AuthResult(false, "User with same email address already exists");

        user = new AppUser
        {
            UserName = email,
            Email = email,
        };

        var created = await userManager.CreateAsync(user, password);
        if (!created.Succeeded)
            return new AuthResult(false, "Unable to create user");

        if (!string.IsNullOrWhiteSpace(roleName))
        {
            if(await roleManager.RoleExistsAsync(roleName))
            {
                if (!await userManager.IsInRoleAsync(user, roleName))
                    await userManager.AddToRoleAsync(user, roleName);
            }
        }

        return new AuthResult(true, "User was created successfully");
    }

    public async Task<AuthResult> SignInUserAsync(string email, string password, bool rememberMe)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return new AuthResult(false, "Incorrect email address or password");

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return new AuthResult(false, "Incorrect email address or password");

        var result = await signInManager.PasswordSignInAsync(email, password, rememberMe, false);
        if (!result.Succeeded)
            return new AuthResult(false, "Incorrect email address or password");

        if (result.IsLockedOut)
            return new AuthResult(false, "User has been temporary blocked.");

        if (result.IsNotAllowed)
            return new AuthResult(false, "User is not allowed to sign in");

        return new AuthResult(true);
    }


    public Task SignOutUserAsync() => signInManager.SignOutAsync();
}