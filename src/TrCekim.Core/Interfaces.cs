namespace TrCekim.Core;

/// <summary>Stored renditions of a photo.</summary>
public enum PhotoSize
{
    /// <summary>Grid thumbnail.</summary>
    Thumb,

    /// <summary>Lightbox size.</summary>
    Web,

    /// <summary>The uploaded file, kept for printing.</summary>
    Original,
}

/// <summary>Binary storage for photo files. Local disk now, S3-compatible later.</summary>
public interface IPhotoStore
{
    Task SaveAsync(Guid galleryId, Guid photoId, PhotoSize size, Stream content, CancellationToken ct = default);

    /// <summary>Returns null when the file does not exist.</summary>
    Stream? OpenRead(Guid galleryId, Guid photoId, PhotoSize size);

    Task DeletePhotoAsync(Guid galleryId, Guid photoId, CancellationToken ct = default);

    Task DeleteGalleryAsync(Guid galleryId, CancellationToken ct = default);
}

public sealed record ProcessedImage(int Width, int Height, byte[] Thumb, byte[] Web);

public interface IImageProcessor
{
    /// <summary>
    /// Decodes an uploaded image, applies its EXIF orientation and produces JPEG
    /// thumbnail and web renditions. Re-encoding drops all metadata (GPS etc.).
    /// </summary>
    /// <exception cref="InvalidImageException">The stream is not a supported image.</exception>
    ProcessedImage Process(Stream input);
}

public sealed class InvalidImageException(string message) : Exception(message);
