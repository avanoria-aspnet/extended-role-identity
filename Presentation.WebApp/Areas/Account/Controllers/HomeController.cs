using Application.Abstractions.Account;
using Application.Abstractions.Authentication;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.WebApp.Areas.Account.Models;
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





    [HttpGet("about-me")]
    public async Task<IActionResult> AboutMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(userId))
        {
            var account = await accountService.GetUserAccountAsync(userId);
            var viewModel = new AboutMeViewModel
            {
                AboutMeForm = new AboutMeForm
                {
                    FirstName = account.Details?.FirstName ?? "",
                    LastName = account.Details?.LastName ?? "",
                    Email = account.Details?.Email ?? "",
                    PhoneNumber = account.Details?.PhoneNumber ?? ""
                },
                ProfileImageUrl = account.Details?.ImageUrl ?? "~/images/profile-image-avatar.png"
            };

            return View(viewModel);
        }

        await authService.SignOutUserAsync();
        return Redirect("/");    
    }


    [HttpPost("about-me")]
    public async Task<IActionResult> AboutMe(AboutMeViewModel viewModel)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return RedirectToAction(nameof(SignOut));

        if (!ModelState.IsValid)
            return View(viewModel);

        var details = new UpdateAccountDetails(
            userId,
            viewModel.AboutMeForm.Email,
            viewModel.AboutMeForm.FirstName,
            viewModel.AboutMeForm.LastName,
            viewModel.AboutMeForm.PhoneNumber
        );

        var result = await accountService.UpdateUserAccountDetailsAsync(details);
        if (!result.Succeeded)
        {
            viewModel.Message = "Unable to save changes";
            return View(viewModel);
        }

        return RedirectToAction(nameof(AboutMe));

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
