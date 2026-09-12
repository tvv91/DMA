using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Web.Enums;
using Web.Interfaces;
using Web.Models;
using Web.Response;
using Web.ViewModels;

namespace Web.Features.Supporting;

public sealed record SearchQuery(EntityType EntityType, string Value) : IRequest<List<AutocompleteResponse>>;
public sealed record StatisticQuery : IRequest<StatisticViewModel?>;
public sealed record AboutQuery : IRequest<Unit>;
public sealed record AccessDeniedQuery : IRequest<Unit>;
public sealed record LoginCommand(LoginViewModel Model, bool IsAjax) : IRequest<LoginResult>;
public sealed record LogoutCommand : IRequest<Unit>;
public sealed record ImageUploadCommand(IFormFile? File) : IRequest<ImageUploadResult>;
public sealed record DeleteTempImageCommand(string Filename) : IRequest<bool>;
public sealed record ImageUploadResult(bool Success, string? Filename, string? Error);
public sealed record LoginResult(bool Success, bool IsAjax, string RedirectUrl);

public sealed class SearchQueryHandler(ISearchService searchService) : IRequestHandler<SearchQuery, List<AutocompleteResponse>>
{
    public Task<List<AutocompleteResponse>> Handle(SearchQuery request, CancellationToken cancellationToken) => searchService.SearchAsync(request.EntityType, request.Value);
}

public sealed class AccessDeniedQueryHandler : IRequestHandler<AccessDeniedQuery, Unit>
{
    public Task<Unit> Handle(AccessDeniedQuery request, CancellationToken cancellationToken) => Task.FromResult(Unit.Value);
}

public sealed class StatisticQueryHandler(IStatisticService statisticService) : IRequestHandler<StatisticQuery, StatisticViewModel?>
{
    public async Task<StatisticViewModel?> Handle(StatisticQuery request, CancellationToken cancellationToken)
    {
        var result = await statisticService.ProcessAsync();
        var vm = JsonSerializer.Deserialize<StatisticViewModel>(result.Data);
        if (vm is not null) vm.LastUpdate = result.LastUpdate;
        return vm;
    }
}

public sealed class AboutQueryHandler : IRequestHandler<AboutQuery, Unit>
{
    public Task<Unit> Handle(AboutQuery request, CancellationToken cancellationToken) => Task.FromResult(Unit.Value);
}

public sealed class LoginCommandHandler(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var model = request.Model;
        if (string.IsNullOrWhiteSpace(model.Email)) return Invalid(request);
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user is null) return Invalid(request);
        var result = await signInManager.PasswordSignInAsync(user.UserName!, model.Password, model.RememberMe, false);
        if (!result.Succeeded) return Invalid(request);
        return new(true, request.IsAjax, string.IsNullOrWhiteSpace(model.ReturnUrl) ? "/" : model.ReturnUrl);
    }

    private static LoginResult Invalid(LoginCommand request) => new(false, request.IsAjax, string.IsNullOrWhiteSpace(request.Model.ReturnUrl) ? "/" : request.Model.ReturnUrl);
}

public sealed class LogoutCommandHandler(SignInManager<ApplicationUser> signInManager) : IRequestHandler<LogoutCommand, Unit>
{
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();
        return Unit.Value;
    }
}

public sealed class ImageUploadCommandHandler(IWebHostEnvironment environment, ILogger<ImageUploadCommandHandler> logger) : IRequestHandler<ImageUploadCommand, ImageUploadResult>
{
    private const long MaxImageSizeBytes = 5 * 1024 * 1024;
    private static readonly HashSet<string> ContentTypes = new(StringComparer.OrdinalIgnoreCase) { "image/jpeg", "image/png" };
    private static readonly Dictionary<string, string> Extensions = new(StringComparer.OrdinalIgnoreCase) { [".jpg"] = ".jpg", [".jpeg"] = ".jpg", [".png"] = ".png" };

    public async Task<ImageUploadResult> Handle(ImageUploadCommand request, CancellationToken cancellationToken)
    {
        var file = request.File;
        if (file is null) return new(false, null, "No image file was provided.");
        if (file.Length <= 0 || file.Length > MaxImageSizeBytes) return new(false, null, "Image file size is invalid.");
        if (!ContentTypes.Contains(file.ContentType)) return new(false, null, "Only JPEG and PNG images are supported.");
        var extension = GetExtension(file.FileName);
        if (extension is null || !await ValidSignature(file, extension)) return new(false, null, "Image file content is invalid.");
        var directory = GetTempDirectory();
        Directory.CreateDirectory(directory);
        var filename = $"{Guid.NewGuid():N}{extension}";
        try
        {
            await using var stream = new FileStream(Path.Combine(directory, filename), FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true);
            await file.CopyToAsync(stream, cancellationToken);
            await stream.FlushAsync(cancellationToken);
            return new(true, filename, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during image upload");
            return new(false, null, "Failed to upload image.");
        }
    }

    private string GetTempDirectory() => Path.GetFullPath(Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "temp"));
    private static string? GetExtension(string name) => Extensions.TryGetValue(Path.GetExtension(Path.GetFileName(name)), out var extension) ? extension : null;
    private static async Task<bool> ValidSignature(IFormFile file, string extension)
    {
        var buffer = new byte[8];
        await using var stream = file.OpenReadStream();
        var read = await stream.ReadAsync(buffer);
        return extension == ".jpg" ? read >= 3 && buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF : read >= 8 && buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 && buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A;
    }
}

public sealed class DeleteTempImageCommandHandler(IWebHostEnvironment environment, ILogger<DeleteTempImageCommandHandler> logger) : IRequestHandler<DeleteTempImageCommand, bool>
{
    public Task<bool> Handle(DeleteTempImageCommand request, CancellationToken cancellationToken)
    {
        var safe = Path.GetFileName(request.Filename);
        if (!string.Equals(safe, request.Filename, StringComparison.Ordinal) || !Guid.TryParseExact(Path.GetFileNameWithoutExtension(safe), "N", out _)) return Task.FromResult(false);
        var path = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "temp", safe);
        try { if (File.Exists(path)) File.Delete(path); return Task.FromResult(true); }
        catch (Exception ex) { logger.LogError(ex, "Error during temp image deleting {Filename}", request.Filename); return Task.FromResult(false); }
    }
}
