using System.Security.Claims;

public static class UserContext
{
    private static IHttpContextAccessor? _httpContextAccessor;

    public static void Initialize(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public static string Id => _httpContextAccessor?.HttpContext?.User?.FindFirst("userid")?.Value ?? string.Empty;
    public static string Username => _httpContextAccessor?.HttpContext?.User?.FindFirst("username")?.Value ?? string.Empty;
    public static string Email => _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
    public static bool IsAdmin => bool.TryParse(_httpContextAccessor?.HttpContext?.User?.FindFirst("isAdmin")?.Value, out var isAdmin) && isAdmin;
}
