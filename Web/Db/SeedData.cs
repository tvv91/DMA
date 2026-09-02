using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Common;

namespace Web.Db;

public static class SeedData
{
    public static async Task EnsurePopulated(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();

        await SeedRolesAsync(services);
        await SeedAdminUserAsync(services, configuration);

        var ctx = services.GetRequiredService<Context>();
        await ctx.Database.MigrateAsync();
        if (!await ctx.Albums.AnyAsync())
        {
            await ctx.Albums.AddRangeAsync(new TestData().GetAlbums());
        }
        if (!await ctx.PostCategories.AnyAsync())
        {
            await ctx.PostCategories.AddRangeAsync(new TestData().GetPosts());
        }
        await ctx.SaveChangesAsync();
    }

    private static async Task SeedRolesAsync(IServiceProvider services)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        foreach (var roleName in new[] { RoleNames.Admin, RoleNames.User })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    private static async Task SeedAdminUserAsync(IServiceProvider services, IConfiguration configuration)
    {
        var adminEmail = configuration["Identity:AdminEmail"];
        var adminPassword = configuration["Identity:AdminPassword"];

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            return;

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
        };

        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, RoleNames.Admin);
    }
}
