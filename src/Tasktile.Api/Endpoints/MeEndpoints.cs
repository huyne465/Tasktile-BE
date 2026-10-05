using System.Security.Claims;
using Tasktile.Api.Auth;

namespace Tasktile.Api.Endpoints;

public static class MeEndpoints
{
    /// Dùng để test token từ app: trả về user id và các claim.
    public static void MapMeEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/me", (ClaimsPrincipal user) => Results.Ok(new
        {
            userId = user.GetUserId(),
            claims = user.Claims.Select(c => new { c.Type, c.Value })
        }))
        .RequireAuthorization();
    }
}
