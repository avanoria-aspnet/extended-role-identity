using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Presentation.WebApp.Areas.Authentication.Models;

namespace Presentation.WebApp.Areas.Authentication.Controllers;

[Area("Authentication")]
public class HomeController(IAuthService authService) : Controller
{
    const string SessionEmailKey = "session_email";

    [HttpGet("registration/setup")]
    public IActionResult Index()
    {
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return (redirect);

        return View();
    }

    [HttpPost("registration/setup")]
    public async Task<IActionResult> Index(SetEmailForm form)
    {
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return (redirect);

        if (!ModelState.IsValid)
            return View(form);

        var result = await authService.CheckIfUserExistsAsync(form.Email);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(nameof(form.ErrorMessage), result.ErrorMessage ?? "A user with same email already exists");
            return View(form);
        }

        HttpContext.Session.SetString(SessionEmailKey, form.Email);

        return RedirectToAction(nameof(SetPassword));
    }


    [HttpGet("registration/set-password")]
    public IActionResult SetPassword()
    {
        var email = HttpContext.Session.GetString(SessionEmailKey);
        if (string.IsNullOrWhiteSpace(email))
            return RedirectToAction(nameof(Index));

        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return (redirect);

        return View();
    }

    [HttpPost("registration/set-password")]
    public async Task<IActionResult> SetPassword(SetPasswordForm form)
    {
        var email = HttpContext.Session.GetString(SessionEmailKey);
        if (string.IsNullOrWhiteSpace(email))
            return RedirectToAction(nameof(Index));

        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return (redirect);

        if (!ModelState.IsValid)
            return View(form);

        var created = await authService.SignUpUserAsync(email, form.Password, "Member");
        if (!created.Succeeded)
        {
            ModelState.AddModelError(nameof(form.ErrorMessage), created.ErrorMessage ?? "Something went wrong");
            return View(form);
        }

        var signedin = await authService.SignInUserAsync(email, form.Password, false);
        return signedin.Succeeded
            ? Redirect("/me")
            : RedirectToAction(nameof(SignIn));
    }

    [HttpGet("sign-in")]
    public IActionResult SignIn(string? returnUrl = null)
    {
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return (redirect);

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn(SignInForm form, string? returnUrl = null)
    {
        var redirect = RedirectWhenSignedIn;
        if (redirect is not null)
            return (redirect);

        if (ModelState.IsValid)
        {
            var signedin = await authService.SignInUserAsync(form.Email, form.Password, form.RememberMe);
            if (signedin.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect(returnUrl);

                return RedirectWhenSignedIn!;
            }
        }

        ViewBag.ReturnUrl = returnUrl;
        ModelState.AddModelError(nameof(form.ErrorMessage), "Incorrect email address or password");
        return View(form);
    }

    public string? FirstName { get; set; }



    private IActionResult? RedirectWhenSignedIn
    {
        get
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return Redirect("/admin");

                if (User.IsInRole("Member"))
                    return Redirect("/me");

                return Redirect("/");
            }

            return null;
        }
    }
}


