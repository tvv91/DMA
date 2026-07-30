using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.ViewModels;

namespace Web.Controllers;

public class AccountController(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    [HttpGet("account/login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        var redirect = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
        var separator = redirect.Contains('?') ? "&" : "?";
        return Redirect($"{redirect}{separator}showLogin=true");
    }

    [HttpPost("account/login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            if (IsAjaxRequest())
                return BadRequest(new { error = "Invalid login attempt." });

            return RedirectToLogin(model.ReturnUrl);
        }

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            if (IsAjaxRequest())
                return BadRequest(new { error = "Invalid login attempt." });

            return RedirectToLogin(model.ReturnUrl);
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var redirectUrl = string.IsNullOrWhiteSpace(model.ReturnUrl) ? "/" : model.ReturnUrl;
            if (IsAjaxRequest())
                return Ok(new { redirectUrl });

            return LocalRedirect(redirectUrl);
        }

        if (IsAjaxRequest())
            return BadRequest(new { error = "Invalid login attempt." });

        return RedirectToLogin(model.ReturnUrl);
    }

    [HttpPost("account/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Post");
    }

    [HttpGet("account/accessdenied")]
    public IActionResult AccessDenied() => View();

    private bool IsAjaxRequest() =>
        string.Equals(Request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

    private IActionResult RedirectToLogin(string? returnUrl)
    {
        var redirect = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
        var separator = redirect.Contains('?') ? "&" : "?";
        return Redirect($"{redirect}{separator}showLogin=true");
    }
}
