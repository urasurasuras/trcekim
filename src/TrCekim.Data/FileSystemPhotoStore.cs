using TrCekim.Core;

namespace TrCekim.Data;

/// <summary>
/// Stores photos under <c>{root}/{galleryId}/{photoId}/{size}.jpg</c>. Paths are built
/// only from GUIDs and an enum, so no user input reaches the file system.
/// </summary>
public sealed class FileSystemPhotoStore(string root) : IPhotoStore
{
    private readonly string _root = Path.GetFullPath(root);

    public async Task SaveAsync(Guid galleryId, Guid photoId, PhotoSize size, Stream content, CancellationToken ct = default)
    {
        var path = FilePath(galleryId, photoId, size);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        // Write to a temp file first so readers never see a half-written photo.
        var tmp = path + ".tmp";
        await using (var fs = File.Create(tmp))
        {
            await content.CopyToAsync(fs, ct);
        }
        File.Move(tmp, path, overwrite: true);
    }

    public Stream? OpenRead(Guid galleryId, Guid photoId, PhotoSize size)
    {
        var path = FilePath(galleryId, photoId, size);
        return File.Exists(path)
            ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 64 * 1024, useAsync: true)
            : null;
    }

    public Task DeletePhotoAsync(Guid galleryId, Guid photoId, CancellationToken ct = default)
    {
        var dir = Path.Combine(_root, galleryId.ToString("N"), photoId.ToString("N"));
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, recursive: true);
        }
        return Task.CompletedTask;
    }

    public Task DeleteGalleryAsync(Guid galleryId, CancellationToken ct = default)
    {
        var dir = Path.Combine(_root, galleryId.ToString("N"));
        if (Directory.Exists(dir))
        {
            Directory.Delete(dir, recursive: true);
        }
        return Task.CompletedTask;
    }

    private string FilePath(Guid galleryId, Guid photoId, PhotoSize size) =>
        Path.Combine(_root, galleryId.ToString("N"), photoId.ToString("N"), size.ToString().ToLowerInvariant() + ".jpg");
}
