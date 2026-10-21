using System.Security.Claims;

namespace ExpenseTracker.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue("nameid")
                    ?? user.FindFirstValue("sub");

        if (!int.TryParse(value, out var id))
            throw new UnauthorizedAccessException("User id not found in token.");

        return id;
    }
}