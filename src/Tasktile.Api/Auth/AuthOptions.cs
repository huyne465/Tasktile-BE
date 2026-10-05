namespace Tasktile.Api.Auth;

public sealed class AuthOptions
{
    /// Firebase: https://securetoken.google.com/{projectId}
    /// Apple:    https://appleid.apple.com
    public string Authority { get; set; } = string.Empty;

    /// Firebase: {projectId}
    /// Apple:    bundle id của app, ví dụ com.huybuu.tasktile
    public string Audience { get; set; } = string.Empty;
}
