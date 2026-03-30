using Application.Abstractions.Account;
using Application.Common.Results;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Services;

public class IdentityAccountService(UserManager<AppUser> userManager) : IAccountService
{
    public async Task<AccountResult> DeleteUserAccountAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return AccountResult.NotFound();

        var deleted = await userManager.DeleteAsync(user);
        return deleted.Succeeded
            ? AccountResult.Ok()
            : AccountResult.Failed(deleted.Errors.FirstOrDefault()?.Description ?? "Unable to delete account");

    }
}
