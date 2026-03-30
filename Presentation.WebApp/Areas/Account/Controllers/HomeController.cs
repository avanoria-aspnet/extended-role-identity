using Application.Abstractions.Account;
using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.WebApp.Areas.Account.Controllers;

[Area("Account")]
[Route("me")]
[Authorize]
public class HomeController(IAuthService authService, IAccountService accountService) : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction(nameof(AboutMe));
    }

    [HttpGet("my-account")]
    public IActionResult AboutMe()
    {
        return View();
    }

    [HttpGet("sign-out")]
    public new async Task<IActionResult> SignOut()
    {
        await authService.SignOutUserAsync();
        return Redirect("/");
    }

    [HttpGet("remove-account")]
    public async Task<IActionResult> RemoveAccount()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var deleted = await accountService.DeleteUserAccountAsync(userId);
            if (!deleted.Succeeded)
            {
                ViewBag.Message = deleted.ErrorMessage;
                return View();
            }
        }

        await authService.SignOutUserAsync();
        return Redirect("/");
    }
}
