using Microsoft.EntityFrameworkCore;
using TrCekim.Core;
using TrCekim.Data;

namespace TrCekim.Web.Services;

public static class PhotoEndpoints
{
    /// <summary>
    /// Photos are never in wwwroot. Every request checks that the visitor unlocked the
    /// gallery (or is an admin), so image URLs cannot be guessed or shared around.
    /// </summary>
    public static IEndpointRouteBuilder MapPhotoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/foto/{photoId:guid}/{size}", async (
            Guid photoId, string size, HttpContext http,
            IDbContextFactory<AppDbContext> dbFactory, IPhotoStore store, GalleryAccess access, TimeProvider clock) =>
        {
            if (!Enum.TryParse<PhotoSize>(size, ignoreCase: true, out var photoSize))
            {
                return Results.NotFound();
            }

            await using var db = await dbFactory.CreateDbContextAsync();
            var photo = await db.Photos.AsNoTracking()
                .Where(p => p.Id == photoId)
                .Select(p => new { p.GalleryId, p.Number, p.Gallery!.IsPublished, p.Gallery.ExpiresAt })
                .FirstOrDefaultAsync();
            if (photo is null)
            {
                return Results.NotFound();
            }

            var isAdmin = http.User.Identity?.IsAuthenticated == true;
            var open = photo.IsPublished && (photo.ExpiresAt is null || photo.ExpiresAt > clock.GetUtcNow());
            if (!isAdmin && (!open || photoSize == PhotoSize.Original || !access.CanView(http, photo.GalleryId)))
            {
                return Results.NotFound(); // do not reveal that the photo exists
            }

            var stream = store.OpenRead(photo.GalleryId, photoId, photoSize);
            if (stream is null)
            {
                return Results.NotFound();
            }

            http.Response.Headers.CacheControl = "private, max-age=86400";
            http.Response.Headers["X-Robots-Tag"] = "noindex";
            return Results.File(stream, "image/jpeg",
                fileDownloadName: photoSize == PhotoSize.Original ? $"{photo.Number}.jpg" : null);
        });

        return app;
    }
}
