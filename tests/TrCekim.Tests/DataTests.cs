using SkiaSharp;
using TrCekim.Core;
using TrCekim.Data;

namespace TrCekim.Tests;

public class SkiaImageProcessorTests
{
    internal static byte[] MakeImage(int width, int height, SKEncodedImageFormat format = SKEncodedImageFormat.Jpeg)
    {
        using var bmp = new SKBitmap(width, height);
        using (var canvas = new SKCanvas(bmp))
        {
            canvas.Clear(SKColors.SteelBlue);
        }
        using var img = SKImage.FromBitmap(bmp);
        using var data = img.Encode(format, 90);
        return data.ToArray();
    }

    [Fact]
    public void Produces_downscaled_jpeg_renditions()
    {
        var result = new SkiaImageProcessor().Process(new MemoryStream(MakeImage(3000, 2000)));

        Assert.Equal(3000, result.Width);
        Assert.Equal(2000, result.Height);
        using var thumb = SKBitmap.Decode(result.Thumb);
        using var web = SKBitmap.Decode(result.Web);
        Assert.Equal(SkiaImageProcessor.ThumbMaxEdge, thumb.Width);
        Assert.Equal(320, thumb.Height);
        Assert.Equal(SkiaImageProcessor.WebMaxEdge, web.Width);
        Assert.Equal(0xFF, result.Web[0]); // JPEG SOI marker
        Assert.Equal(0xD8, result.Web[1]);
    }

    [Fact]
    public void Does_not_upscale_small_images()
    {
        var result = new SkiaImageProcessor().Process(new MemoryStream(MakeImage(300, 200, SKEncodedImageFormat.Png)));
        using var web = SKBitmap.Decode(result.Web);
        Assert.Equal(300, web.Width);
        Assert.Equal(200, web.Height);
    }

    [Fact]
    public void Rejects_non_images()
    {
        Assert.Throws<InvalidImageException>(() =>
            new SkiaImageProcessor().Process(new MemoryStream("not an image"u8.ToArray())));
    }

    // 2×1 source: red at (0,0), green at (1,0).
    [Theory]
    [InlineData(SKEncodedOrigin.TopLeft, 2, 1, 0, 0)]
    [InlineData(SKEncodedOrigin.TopRight, 2, 1, 1, 0)]     // mirrored: red ends up right
    [InlineData(SKEncodedOrigin.BottomRight, 2, 1, 1, 0)]  // 180°
    [InlineData(SKEncodedOrigin.BottomLeft, 2, 1, 0, 0)]   // vertical flip of one row
    [InlineData(SKEncodedOrigin.LeftTop, 1, 2, 0, 0)]      // transpose
    [InlineData(SKEncodedOrigin.RightTop, 1, 2, 0, 0)]     // 90° CW: top row becomes right column
    [InlineData(SKEncodedOrigin.RightBottom, 1, 2, 0, 1)]  // transverse
    [InlineData(SKEncodedOrigin.LeftBottom, 1, 2, 0, 1)]   // 90° CCW: red goes to the bottom
    public void ApplyOrientation_maps_pixels(SKEncodedOrigin origin, int w, int h, int redX, int redY)
    {
        using var src = new SKBitmap(2, 1);
        src.SetPixel(0, 0, SKColors.Red);
        src.SetPixel(1, 0, SKColors.Lime);

        using var dst = SkiaImageProcessor.ApplyOrientation(src, origin);

        Assert.Equal(w, dst.Width);
        Assert.Equal(h, dst.Height);
        Assert.Equal(SKColors.Red, dst.GetPixel(redX, redY));
    }
}

public sealed class FileSystemPhotoStoreTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "trc-store-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task Save_open_and_delete()
    {
        var store = new FileSystemPhotoStore(_root);
        var g = Guid.NewGuid();
        var p = Guid.NewGuid();

        await store.SaveAsync(g, p, PhotoSize.Thumb, new MemoryStream([1, 2, 3]));
        using (var s = store.OpenRead(g, p, PhotoSize.Thumb))
        {
            Assert.NotNull(s);
            var ms = new MemoryStream();
            await s!.CopyToAsync(ms);
            Assert.Equal(new byte[] { 1, 2, 3 }, ms.ToArray());
        }
        Assert.Null(store.OpenRead(g, p, PhotoSize.Web));

        await store.DeletePhotoAsync(g, p);
        Assert.Null(store.OpenRead(g, p, PhotoSize.Thumb));

        await store.SaveAsync(g, p, PhotoSize.Web, new MemoryStream([9]));
        await store.DeleteGalleryAsync(g);
        Assert.False(Directory.Exists(Path.Combine(_root, g.ToString("N"))));
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }
}
