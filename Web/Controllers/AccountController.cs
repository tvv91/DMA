using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Web.Features.Supporting;
using Web.ViewModels;

namespace Web.Controllers;

public class AccountController(ISender sender) : Controller
{
    private readonly ISender _sender = sender;

    [HttpGet("account/login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        var redirect = GetLocalReturnUrl(returnUrl);
        var separator = redirect.Contains('?') ? "&" : "?";
        return Redirect($"{redirect}{separator}showLogin=true");
    }

    [HttpPost("account/login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var isAjax = IsAjaxRequest();
        if (!ModelState.IsValid)
            return InvalidLogin(model.ReturnUrl, isAjax);

        var result = await _sender.Send(new LoginCommand(model, isAjax));
        if (!result.Success)
            return InvalidLogin(model.ReturnUrl, isAjax);
        var redirectUrl = GetLocalReturnUrl(result.RedirectUrl);
        return result.IsAjax ? Ok(new { redirectUrl }) : LocalRedirect(redirectUrl);
    }

    [HttpPost("account/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _sender.Send(new LogoutCommand());
        return RedirectToAction("Index", "Post");
    }

    [HttpGet("account/accessdenied")]
    public async Task<IActionResult> AccessDenied()
    {
        await _sender.Send(new AccessDeniedQuery());
        return View();
    }

    private bool IsAjaxRequest() => string.Equals(Request.Headers.XRequestedWith, "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

    private IActionResult InvalidLogin(string? returnUrl, bool isAjax) => isAjax
        ? BadRequest(new { error = "Invalid login attempt." })
        : RedirectToLogin(returnUrl);

    private IActionResult RedirectToLogin(string? returnUrl)
    {
        var redirect = GetLocalReturnUrl(returnUrl);
        var separator = redirect.Contains('?') ? "&" : "?";
        return Redirect($"{redirect}{separator}showLogin=true");
    }

    private string GetLocalReturnUrl(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? returnUrl
            : "/";
}
