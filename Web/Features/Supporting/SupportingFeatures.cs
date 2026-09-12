using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Infrastructure.Persistence;
using Web.Enums;
using Web.Models;
using Web.Infrastructure.Storage;
using Web.ViewModels;

namespace Web.Features.Supporting;

public sealed record StatisticQuery : IRequest<StatisticViewModel?>;
public sealed record AboutQuery : IRequest<Unit>;
public sealed record AccessDeniedQuery : IRequest<Unit>;
public sealed record LoginCommand(LoginViewModel Model, bool IsAjax) : IRequest<LoginResult>;
public sealed record LogoutCommand : IRequest<Unit>;
public sealed record ImageUploadCommand(IFormFile? File) : IRequest<ImageUploadResult>;
public sealed record DeleteTempImageCommand(string Filename) : IRequest<bool>;
public sealed record ImageUploadResult(bool Success, string? Filename, string? Error);
public sealed record LoginResult(bool Success, bool IsAjax, string RedirectUrl);

public sealed class AccessDeniedQueryHandler : IRequestHandler<AccessDeniedQuery, Unit>
{
    public Task<Unit> Handle(AccessDeniedQuery request, CancellationToken cancellationToken) => Task.FromResult(Unit.Value);
}

public sealed class StatisticQueryHandler(Context context, TimeProvider timeProvider) : IRequestHandler<StatisticQuery, StatisticViewModel?>
{
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);
    private static DateTime? lastRefreshAttempt;
    private static readonly TimeSpan RefreshCooldown = TimeSpan.FromMinutes(5);
    private static readonly double[] DsdFrequencies = [2.8, 5.6, 11.2, 22.5];

    public async Task<StatisticViewModel?> Handle(StatisticQuery request, CancellationToken cancellationToken)
    {
        var result = await ProcessAsync(cancellationToken);
        var vm = JsonSerializer.Deserialize<StatisticViewModel>(result.Data);
        if (vm is not null) vm.LastUpdate = result.LastUpdate;
        return vm;
    }

    private async Task<Statistic> ProcessAsync(CancellationToken cancellationToken)
    {
        var statistic = await context.Statistics.FirstOrDefaultAsync(cancellationToken);
        var now = timeProvider.GetUtcNow().UtcDateTime;
        var refresh = statistic is null || now - statistic.LastUpdate > TimeSpan.FromDays(1) &&
            (lastRefreshAttempt is null || now - lastRefreshAttempt.Value > RefreshCooldown);
        if (!refresh) return statistic!;

        await RefreshLock.WaitAsync(cancellationToken);
        try
        {
            statistic = await context.Statistics.FirstOrDefaultAsync(cancellationToken);
            now = timeProvider.GetUtcNow().UtcDateTime;
            if (statistic is not null && now - statistic.LastUpdate <= TimeSpan.FromDays(1)) return statistic;
            lastRefreshAttempt = now;
            var counters = new StatisticCounters
            {
                TotalAlbums = await context.Albums.CountAsync(cancellationToken),
                TotalSize = await context.Releases.Where(x => x.Size != null).SumAsync(x => x.Size ?? 0, cancellationToken),
                StorageCount = await context.Storages.CountAsync(cancellationToken),
                TotalReleases = await context.Releases.CountAsync(cancellationToken),
                TotalArtists = await context.Artists.CountAsync(cancellationToken),
                TotalEquipment = await context.Adces.CountAsync(cancellationToken) + await context.Amplifiers.CountAsync(cancellationToken) + await context.Cartridges.CountAsync(cancellationToken) + await context.Players.CountAsync(cancellationToken) + await context.Wires.CountAsync(cancellationToken),
                Genre = await CounterList(context.Genres.Select(x => new CounterItem { Description = x.Name, Count = x.Albums.Count }), cancellationToken),
                Artist = await CounterList(context.Artists.Select(x => new CounterItem { Description = x.Name, Count = x.Albums.Count }), cancellationToken),
                Year = await CounterList(context.Years.Select(x => new CounterItem { Description = x.Value.ToString(), Count = x.Releases.Count }), cancellationToken),
                Country = await CounterList(context.Countries.Select(x => new CounterItem { Description = x.Name, Count = x.Releases.Count }), cancellationToken),
                Label = await CounterList(context.Labels.Select(x => new CounterItem { Description = x.Name, Count = x.Releases.Count }), cancellationToken),
                Bitness = await CounterList(context.Bitnesses.Select(x => new CounterItem { Description = x.Value + " bit", Count = context.Releases.Count(r => r.FormatInfo != null && r.FormatInfo.BitnessId == x.Id) }), cancellationToken),
                Sampling = await CounterList(context.Samplings.Select(x => new CounterItem { Description = x.Value + (DsdFrequencies.Contains(x.Value) ? " MHz" : " kHz"), Count = context.Releases.Count(r => r.FormatInfo != null && r.FormatInfo.SamplingId == x.Id) }), cancellationToken),
                SourceFormat = await CounterList(context.SourceFormats.Select(x => new CounterItem { Description = x.Name, Count = context.Releases.Count(r => r.FormatInfo != null && r.FormatInfo.SourceFormatId == x.Id) }), cancellationToken),
                DigitalFormat = await CounterList(context.DigitalFormats.Select(x => new CounterItem { Description = x.Name, Count = context.Releases.Count(r => r.FormatInfo != null && r.FormatInfo.DigitalFormatId == x.Id) }), cancellationToken),
                VinylState = await CounterList(context.VinylStates.Select(x => new CounterItem { Description = x.Name, Count = context.Releases.Count(r => r.FormatInfo != null && r.FormatInfo.VinylStateId == x.Id) }), cancellationToken),
                Adc = await EquipmentCounters(context.Adces.Select(x => new { x.Name, Manufacturer = x.Manufacturer == null ? null : x.Manufacturer.Name, Count = context.Releases.Count(r => r.EquipmentInfo != null && r.EquipmentInfo.AdcId == x.Id) }), cancellationToken),
                Amplifier = await EquipmentCounters(context.Amplifiers.Select(x => new { x.Name, Manufacturer = x.Manufacturer == null ? null : x.Manufacturer.Name, Count = context.Releases.Count(r => r.EquipmentInfo != null && r.EquipmentInfo.AmplifierId == x.Id) }), cancellationToken),
                Cartridge = await EquipmentCounters(context.Cartridges.Select(x => new { x.Name, Manufacturer = x.Manufacturer == null ? null : x.Manufacturer.Name, Count = context.Releases.Count(r => r.EquipmentInfo != null && r.EquipmentInfo.CartridgeId == x.Id) }), cancellationToken),
                Player = await EquipmentCounters(context.Players.Select(x => new { x.Name, Manufacturer = x.Manufacturer == null ? null : x.Manufacturer.Name, Count = context.Releases.Count(r => r.EquipmentInfo != null && r.EquipmentInfo.PlayerId == x.Id) }), cancellationToken),
                Wire = await EquipmentCounters(context.Wires.Select(x => new { x.Name, Manufacturer = x.Manufacturer == null ? null : x.Manufacturer.Name, Count = context.Releases.Count(r => r.EquipmentInfo != null && r.EquipmentInfo.WireId == x.Id) }), cancellationToken)
            };
            statistic ??= new Statistic();
            statistic.Data = JsonSerializer.Serialize(counters);
            statistic.LastUpdate = now;
            if (context.Entry(statistic).State == EntityState.Detached) context.Statistics.Add(statistic);
            await context.SaveChangesAsync(cancellationToken);
            return statistic;
        }
        finally { RefreshLock.Release(); }
    }

    private static Task<List<CounterItem>> CounterList(IQueryable<CounterItem> query, CancellationToken token) => query.Where(x => x.Count > 0).OrderByDescending(x => x.Count).Take(10).ToListAsync(token);
    private static async Task<List<CounterItem>> EquipmentCounters<T>(IQueryable<T> query, CancellationToken token) where T : class
    {
        return await query.Select(x => new CounterItem { Description = EF.Property<string>(x, "Name"), Count = EF.Property<int>(x, "Count") }).Where(x => x.Count > 0).OrderByDescending(x => x.Count).Take(10).ToListAsync(token);
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

