namespace DemoPasskeys;

public static class Cookies
{
    private static string CookieName => ".auth";

    public static void AppendAuthCookie(this HttpContext context, string userId)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.Now.AddDays(7)
        };
        context.Response.Cookies.Append(CookieName, userId, cookieOptions);
    }

    public static void DeleteAuthCookie(this HttpContext context)
    {
        context.Response.Cookies.Delete(CookieName);
    }

    public static string? GetAuthCookie(this HttpContext context)
    {
        return context.Request.Cookies[CookieName];
    }
}