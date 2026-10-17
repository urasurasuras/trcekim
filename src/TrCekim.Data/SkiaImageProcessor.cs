using SkiaSharp;
using TrCekim.Core;

namespace TrCekim.Data;

public sealed class SkiaImageProcessor : IImageProcessor
{
    public const int ThumbMaxEdge = 480;
    public const int WebMaxEdge = 1600;
    private const int JpegQuality = 82;

    public ProcessedImage Process(Stream input)
    {
        using var data = SKData.Create(input)
            ?? throw new InvalidImageException("Dosya okunamadı.");
        using var codec = SKCodec.Create(data)
            ?? throw new InvalidImageException("Desteklenmeyen resim biçimi.");

        using var decoded = SKBitmap.Decode(codec)
            ?? throw new InvalidImageException("Resim çözülemedi.");
        using var upright = ApplyOrientation(decoded, codec.EncodedOrigin);

        return new ProcessedImage(
            upright.Width,
            upright.Height,
            Thumb: Encode(upright, ThumbMaxEdge),
            Web: Encode(upright, WebMaxEdge));
    }

    private static byte[] Encode(SKBitmap source, int maxEdge)
    {
        var scale = Math.Min(1.0, (double)maxEdge / Math.Max(source.Width, source.Height));
        var w = Math.Max(1, (int)Math.Round(source.Width * scale));
        var h = Math.Max(1, (int)Math.Round(source.Height * scale));

        using var image = SKImage.FromBitmap(source);
        using var surface = SKSurface.Create(new SKImageInfo(w, h, SKColorType.Rgba8888, SKAlphaType.Premul));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.White); // flatten PNG transparency for JPEG
        canvas.DrawImage(image, new SKRect(0, 0, w, h), new SKSamplingOptions(SKCubicResampler.Mitchell));
        canvas.Flush();

        using var snapshot = surface.Snapshot();
        using var encoded = snapshot.Encode(SKEncodedImageFormat.Jpeg, JpegQuality);
        return encoded.ToArray();
    }

    /// <summary>Rotates/flips pixels so the image is upright without EXIF.</summary>
    internal static SKBitmap ApplyOrientation(SKBitmap src, SKEncodedOrigin origin)
    {
        if (origin == SKEncodedOrigin.TopLeft)
        {
            return src.Copy();
        }

        float w = src.Width, h = src.Height;

        // Maps stored pixel (x, y) to display pixel (x', y'):
        // x' = ScaleX·x + SkewX·y + TransX,  y' = SkewY·x + ScaleY·y + TransY
        var m = origin switch
        {
            SKEncodedOrigin.TopRight => new SKMatrix(-1, 0, w, 0, 1, 0, 0, 0, 1),    // mirror horizontal
            SKEncodedOrigin.BottomRight => new SKMatrix(-1, 0, w, 0, -1, h, 0, 0, 1), // rotate 180
            SKEncodedOrigin.BottomLeft => new SKMatrix(1, 0, 0, 0, -1, h, 0, 0, 1),   // mirror vertical
            SKEncodedOrigin.LeftTop => new SKMatrix(0, 1, 0, 1, 0, 0, 0, 0, 1),       // transpose
            SKEncodedOrigin.RightTop => new SKMatrix(0, -1, h, 1, 0, 0, 0, 0, 1),     // rotate 90° CW
            SKEncodedOrigin.RightBottom => new SKMatrix(0, -1, h, -1, 0, w, 0, 0, 1), // transverse
            SKEncodedOrigin.LeftBottom => new SKMatrix(0, 1, 0, -1, 0, w, 0, 0, 1),   // rotate 90° CCW
            _ => SKMatrix.Identity,
        };

        var swap = m.ScaleX == 0;
        var dst = new SKBitmap(swap ? src.Height : src.Width, swap ? src.Width : src.Height);
        using var canvas = new SKCanvas(dst);
        canvas.SetMatrix(m);
        using var img = SKImage.FromBitmap(src);
        canvas.DrawImage(img, 0, 0, SKSamplingOptions.Default);
        return dst;
    }
}
