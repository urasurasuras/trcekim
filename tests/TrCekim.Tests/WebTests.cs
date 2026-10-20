using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TrCekim.Core;
using TrCekim.Data;

namespace TrCekim.Tests;

public sealed class TrCekimFactory : WebApplicationFactory<Program>
{
    public string DataDir { get; } = Path.Combine(Path.GetTempPath(), "trc-web-" + Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("DataDirectory", DataDir);
        builder.UseSetting("DisableHttpsRedirection", "true");
        builder.UseSetting("RateLimit:PostsPerMinute", "1000");
        builder.UseSetting("Admin:Email", "admin@example.com");
        builder.UseSetting("Admin:Password", "Test-Password-123");
    }

    public async Task<(Gallery Gallery, Photo Photo)> SeedGalleryAsync(bool published = true, DateTimeOffset? expiresAt = null)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var store = scope.ServiceProvider.GetRequiredService<IPhotoStore>();

        var school = new School { Name = "Test Okulu " + Guid.NewGuid().ToString("N")[..6] };
        var gallery = new Gallery
        {
            School = school, Season = "2025-2026", Title = "Mezuniyet", GroupName = "4-A",
            AccessCode = await db.NewAccessCodeAsync(), IsPublished = published, ExpiresAt = expiresAt,
        };
        var photo = new Photo { Gallery = gallery, Number = "017988", SortOrder = 1 };
        db.AddRange(school, gallery, photo);
        await db.SaveChangesAsync();

        var jpeg = SkiaImageProcessorTests.MakeImage(40, 30);
        foreach (var size in Enum.GetValues<PhotoSize>())
        {
            await store.SaveAsync(gallery.Id, photo.Id, size, new MemoryStream(jpeg));
        }
        return (gallery, photo);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        if (Directory.Exists(DataDir))
        {
            Directory.Delete(DataDir, recursive: true);
        }
    }
}

public class WebTests(TrCekimFactory factory) : IClassFixture<TrCekimFactory>
{
    private HttpClient NewClient() => factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    private static async Task<string> TokenAsync(HttpClient client, string url)
    {
        var html = await client.GetStringAsync(url);
        var m = Regex.Match(html, "name=\"__RequestVerificationToken\" value=\"([^\"]+)\"");
        Assert.True(m.Success, "antiforgery token not found on " + url);
        return m.Groups[1].Value;
    }

    private static async Task<HttpResponseMessage> PostFormAsync(HttpClient client, string url, Dictionary<string, string> fields)
    {
        fields["__RequestVerificationToken"] = await TokenAsync(client, url);
        return await client.PostAsync(url, new FormUrlEncodedContent(fields));
    }

    private static Task<HttpResponseMessage> EnterCodeAsync(HttpClient client, string code) =>
        PostFormAsync(client, "/galeri", new() { ["_handler"] = "code", ["Input.Code"] = code });

    [Theory]
    [InlineData("/")]
    [InlineData("/hizmetler")]
    [InlineData("/hakkimizda")]
    [InlineData("/referanslar")]
    [InlineData("/iletisim")]
    [InlineData("/gizlilik")]
    [InlineData("/galeri")]
    [InlineData("/sepet")]
    public async Task Public_pages_render(string url)
    {
        var response = await NewClient().GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("TRÇekim", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Services_page_lists_seeded_legacy_prices()
    {
        var html = await NewClient().GetStringAsync("/hizmetler");
        Assert.Contains("Baskı 15×23", html);
        Assert.Contains("Poster 50×70", html);
    }

    [Fact]
    public async Task Gallery_is_hidden_without_code()
    {
        var (gallery, photo) = await factory.SeedGalleryAsync();
        var client = NewClient();

        var html = await client.GetStringAsync($"/galeri/{gallery.Id}");
        Assert.DoesNotContain(photo.Number, html);
        Assert.Contains("galeri kodunu girin", html);

        var image = await client.GetAsync($"/foto/{photo.Id}/thumb");
        Assert.Equal(HttpStatusCode.NotFound, image.StatusCode);
    }

    [Fact]
    public async Task Correct_code_unlocks_gallery_and_photos()
    {
        var (gallery, photo) = await factory.SeedGalleryAsync();
        var client = NewClient();

        // Typed in lower case with a separator, as a parent would.
        var formatted = AccessCodes.Format(gallery.AccessCode).ToLowerInvariant();
        var response = await EnterCodeAsync(client, formatted);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.EndsWith($"galeri/{gallery.Id}", response.Headers.Location!.ToString());

        var html = await client.GetStringAsync($"/galeri/{gallery.Id}");
        Assert.Contains(photo.Number, html);
        Assert.Contains("noindex", html);

        var thumb = await client.GetAsync($"/foto/{photo.Id}/thumb");
        Assert.Equal(HttpStatusCode.OK, thumb.StatusCode);
        Assert.Equal("image/jpeg", thumb.Content.Headers.ContentType!.MediaType);
        Assert.Contains("private", thumb.Headers.CacheControl!.ToString());

        // Originals are for the photographer only.
        var original = await client.GetAsync($"/foto/{photo.Id}/original");
        Assert.Equal(HttpStatusCode.NotFound, original.StatusCode);
    }

    [Fact]
    public async Task Wrong_code_shows_error()
    {
        var response = await EnterCodeAsync(NewClient(), "ZZZZ-ZZZZ");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("açık bir galeri bulunamadı", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Unpublished_or_expired_gallery_cannot_be_opened()
    {
        var (draft, _) = await factory.SeedGalleryAsync(published: false);
        var (expired, _) = await factory.SeedGalleryAsync(expiresAt: DateTimeOffset.UtcNow.AddDays(-1));

        foreach (var g in new[] { draft, expired })
        {
            var response = await EnterCodeAsync(NewClient(), g.AccessCode);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("açık bir galeri bulunamadı", await response.Content.ReadAsStringAsync());
        }
    }

    [Fact]
    public async Task Parent_can_order_prints()
    {
        var (gallery, photo) = await factory.SeedGalleryAsync();
        var client = NewClient();
        await EnterCodeAsync(client, gallery.AccessCode);

        var add = await PostFormAsync(client, $"/galeri/{gallery.Id}/foto/{photo.Id}", new()
        {
            ["_handler"] = "add", ["Input.ProductId"] = "2", ["Input.Quantity"] = "3",
        });
        Assert.Contains("Sepete eklendi", await add.Content.ReadAsStringAsync());

        var cart = await client.GetStringAsync("/sepet");
        Assert.Contains("017988", cart);
        Assert.Contains("45,00", cart); // 3 × 15 TL

        var checkout = await PostFormAsync(client, "/siparis", new()
        {
            ["_handler"] = "checkout",
            ["Input.StudentName"] = "Ayşe Yılmaz",
            ["Input.ParentName"] = "Fatma Yılmaz",
            ["Input.Relation"] = "Anne",
            ["Input.Phone"] = "05321234567",
            ["Input.Delivery"] = "ViaSchool",
            ["Input.Consent"] = "true",
        });
        Assert.Equal(HttpStatusCode.Redirect, checkout.StatusCode);
        var reference = checkout.Headers.Location!.ToString().Split('/').Last();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var order = await db.Orders.Include(o => o.Lines).SingleAsync(o => o.Reference == reference);
        Assert.Equal(45m, order.Total);
        Assert.Equal("Fatma Yılmaz", order.ParentName);
        var line = Assert.Single(order.Lines);
        Assert.Equal("017988", line.PhotoNumber);
        Assert.Equal("Baskı 20×30", line.ProductName);

        var cartAfter = await client.GetStringAsync("/sepet");
        Assert.Contains("Sepetiniz boş", cartAfter);
    }

    [Fact]
    public async Task Checkout_requires_consent()
    {
        var (gallery, photo) = await factory.SeedGalleryAsync();
        var client = NewClient();
        await EnterCodeAsync(client, gallery.AccessCode);
        await PostFormAsync(client, $"/galeri/{gallery.Id}/foto/{photo.Id}", new()
        {
            ["_handler"] = "add", ["Input.ProductId"] = "1", ["Input.Quantity"] = "1",
        });

        var checkout = await PostFormAsync(client, "/siparis", new()
        {
            ["_handler"] = "checkout", ["Input.StudentName"] = "A", ["Input.ParentName"] = "B",
            ["Input.Phone"] = "05321234567", ["Input.Delivery"] = "ViaSchool",
        });
        Assert.Equal(HttpStatusCode.OK, checkout.StatusCode);
        Assert.Contains("onay gerekli", await checkout.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Cannot_add_photo_from_gallery_without_access()
    {
        var (gallery, photo) = await factory.SeedGalleryAsync();
        var html = await NewClient().GetStringAsync($"/galeri/{gallery.Id}/foto/{photo.Id}");
        Assert.Contains("Fotoğraf bulunamadı", html);
    }

    [Fact]
    public async Task Contact_form_stores_message_and_ignores_bots()
    {
        var client = NewClient();
        var ok = await PostFormAsync(client, "/iletisim", new()
        {
            ["_handler"] = "contact", ["Input.Name"] = "Veli", ["Input.Email"] = "veli@example.com",
            ["Input.Message"] = "Mezuniyet fotoğrafları hakkında bilgi almak istiyorum.",
        });
        Assert.Contains("Mesajınız alındı", await ok.Content.ReadAsStringAsync());

        var bot = await PostFormAsync(client, "/iletisim", new()
        {
            ["_handler"] = "contact", ["Input.Name"] = "Bot", ["Input.Email"] = "bot@example.com",
            ["Input.Message"] = "Buy cheap things now!!!", ["Input.Website"] = "http://spam.example",
        });
        Assert.Contains("Mesajınız alındı", await bot.Content.ReadAsStringAsync());

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.True(await db.ContactMessages.AnyAsync(m => m.Email == "veli@example.com"));
        Assert.False(await db.ContactMessages.AnyAsync(m => m.Email == "bot@example.com"));
    }

    [Theory]
    [InlineData("/admin")]
    [InlineData("/admin/galeriler")]
    [InlineData("/admin/siparisler.csv")]
    public async Task Admin_requires_login(string url)
    {
        var response = await NewClient().GetAsync(url);
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/admin/giris", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task Admin_can_log_in_and_see_originals()
    {
        var (_, photo) = await factory.SeedGalleryAsync(published: false);
        var client = NewClient();

        var bad = await PostFormAsync(client, "/admin/giris", new()
        {
            ["_handler"] = "login", ["Input.Email"] = "admin@example.com", ["Input.Password"] = "wrong-password",
        });
        Assert.Contains("hatalı", await bad.Content.ReadAsStringAsync());

        var login = await PostFormAsync(client, "/admin/giris", new()
        {
            ["_handler"] = "login", ["Input.Email"] = "admin@example.com", ["Input.Password"] = "Test-Password-123",
        });
        Assert.Equal(HttpStatusCode.Redirect, login.StatusCode);

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/admin")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/admin/galeriler")).StatusCode);

        // Admin sees originals even for unpublished galleries.
        var original = await client.GetAsync($"/foto/{photo.Id}/original");
        Assert.Equal(HttpStatusCode.OK, original.StatusCode);

        var csv = await client.GetAsync("/admin/siparisler.csv");
        Assert.Equal(HttpStatusCode.OK, csv.StatusCode);
        Assert.StartsWith("Sipariş;", (await csv.Content.ReadAsStringAsync()).TrimStart('﻿'));
    }
}
