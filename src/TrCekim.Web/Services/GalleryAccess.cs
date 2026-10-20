using Microsoft.AspNetCore.DataProtection;

namespace TrCekim.Web.Services;

/// <summary>
/// Remembers which galleries a visitor unlocked with an access code, in a
/// tamper-proof (Data Protection) cookie. No parent account is needed.
/// </summary>
public sealed class GalleryAccess(IDataProtectionProvider dp)
{
    public const string CookieName = "trc_galleries";
    private const int MaxGalleries = 20;
    private static readonly TimeSpan Lifetime = TimeSpan.FromDays(30);

    private readonly IDataProtector _protector = dp.CreateProtector("TrCekim.GalleryAccess.v1");

    public IReadOnlyList<Guid> Read(HttpContext http)
    {
        if (!http.Request.Cookies.TryGetValue(CookieName, out var raw) || string.IsNullOrEmpty(raw))
        {
            return [];
        }

        try
        {
            return _protector.Unprotect(raw)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty)
                .Where(g => g != Guid.Empty)
                .ToList();
        }
        catch (System.Security.Cryptography.CryptographicException)
        {
            return []; // tampered or keys rotated
        }
    }

    public bool CanView(HttpContext http, Guid galleryId) =>
        http.User.Identity?.IsAuthenticated == true || Read(http).Contains(galleryId);

    public void Grant(HttpContext http, Guid galleryId)
    {
        var ids = Read(http).Where(g => g != galleryId).Prepend(galleryId).Take(MaxGalleries);
        http.Response.Cookies.Append(CookieName, _protector.Protect(string.Join(',', ids)), new CookieOptions
        {
            HttpOnly = true,
            Secure = http.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            MaxAge = Lifetime,
        });
    }
}
