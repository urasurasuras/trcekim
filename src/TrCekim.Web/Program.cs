using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.WebEncoders;
using TrCekim.Data;
using TrCekim.Web.Components;
using TrCekim.Web.Services;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// All runtime state lives under one directory (a Docker volume in production).
var dataDir = Path.GetFullPath(config["DataDirectory"] ?? Path.Combine(builder.Environment.ContentRootPath, "data"));
Directory.CreateDirectory(dataDir);

builder.Services.AddTrCekimData(
    config.GetConnectionString("Default") ?? $"Data Source={Path.Combine(dataDir, "trcekim.db")}",
    Path.Combine(dataDir, "photos"));

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(dataDir, "keys")))
    .SetApplicationName("TrCekim");

builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
builder.Services.AddIdentityCore<IdentityUser>(o =>
    {
        o.Password.RequiredLength = 10;
        o.Lockout.MaxFailedAccessAttempts = 5;
        o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/admin/giris";
    o.AccessDeniedPath = "/admin/giris";
    o.Cookie.Name = "trc_admin";
    o.ExpireTimeSpan = TimeSpan.FromHours(12);
});
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.Configure<SiteOptions>(config.GetSection("Site"));
builder.Services.Configure<SmtpOptions>(config.GetSection("Smtp"));
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<GalleryAccess>();
builder.Services.AddScoped<CartService>();
builder.Services.AddSingleton<Notifier>();
builder.Services.AddHttpContextAccessor();

// Brute-force protection for code entry, contact form and admin login (POSTs only).
builder.Services.AddRateLimiter(o =>
{
    o.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    o.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(http =>
    {
        var path = http.Request.Path.Value ?? "";
        var limited = HttpMethods.IsPost(http.Request.Method) &&
            (path.StartsWith("/galeri", StringComparison.OrdinalIgnoreCase)
             || path.StartsWith("/iletisim", StringComparison.OrdinalIgnoreCase)
             || path.StartsWith("/admin/giris", StringComparison.OrdinalIgnoreCase));
        if (!limited)
        {
            return RateLimitPartition.GetNoLimiter("none");
        }
        var ip = http.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = config.GetValue("RateLimit:PostsPerMinute", 20),
            Window = TimeSpan.FromMinutes(1),
        });
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    if (config.GetValue<bool>("TrustForwardedHeaders"))
    {
        // Behind a reverse proxy on a private network (e.g. Nginx Proxy Manager in Docker).
        o.KnownIPNetworks.Clear();
        o.KnownProxies.Clear();
    }
});

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Emit Turkish characters as-is instead of &#xC7; entities.
builder.Services.Configure<WebEncoderOptions>(o => o.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

var app = builder.Build();

var tr = new CultureInfo("tr-TR");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(tr),
    SupportedCultures = [tr],
    SupportedUICultures = [tr],
});

app.UseForwardedHeaders();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
if (!config.GetValue<bool>("DisableHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapPhotoEndpoints();
app.MapAdminEndpoints();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await Startup.InitializeAsync(app.Services, config, app.Logger);

app.Run();

public partial class Program;
