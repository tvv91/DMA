using Microsoft.AspNetCore.Http;

namespace Web.Authorization;

public sealed class AdminAuthorization(IHttpContextAccessor httpContextAccessor) : IAdminAuthorization
{
    public void EnsureAdmin()
    {
        if (httpContextAccessor.HttpContext?.User.IsInRole(RoleNames.Admin) != true)
            throw new UnauthorizedAccessException("Administrator access is required.");
    }
}
