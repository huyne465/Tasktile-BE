using System.Security.Claims;

namespace Tasktile.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    /// "sub" = Firebase UID hoặc Apple user id. Dùng làm khoá chủ sở hữu dữ liệu.
    public static string GetUserId(this ClaimsPrincipal user) =>
        user.FindFirst("sub")?.Value
        ?? throw new InvalidOperationException("Token không có claim 'sub'.");
}
