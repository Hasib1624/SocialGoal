using System.IO;
using SkiaSharp;

namespace SocialGoal.Web.Services
{
    public static class ImageCropper
    {
        private const int MaxSide = 4096;

        /// <summary>Cuts a rectangle out of an image and returns it as PNG; null if the input is not a usable image.</summary>
        public static byte[] CropToPng(byte[] source, int x, int y, int width, int height)
        {
            if (source == null || source.Length == 0 || width < 1 || height < 1 || width > MaxSide || height > MaxSide)
                return null;

            // check the header before decoding, so a small file cannot demand gigabytes of pixels
            using var data = SKData.CreateCopy(source);
            using var codec = SKCodec.Create(data);
            if (codec == null || codec.Info.Width > MaxSide || codec.Info.Height > MaxSide)
                return null;
            using var original = new SKBitmap(codec.Info.WithColorType(SKColorType.Rgba8888).WithAlphaType(SKAlphaType.Premul));
            // like GDI+ before, accept slightly damaged files that still yield pixels
            var decoded = codec.GetPixels(original.Info, original.GetPixels());
            if (decoded != SKCodecResult.Success && decoded != SKCodecResult.IncompleteInput && decoded != SKCodecResult.ErrorInInput)
                return null;

            using var cropped = new SKBitmap(width, height);
            using (var canvas = new SKCanvas(cropped))
            using (var image = SKImage.FromBitmap(original))
            {
                canvas.Clear(SKColors.Transparent);
                canvas.DrawImage(image, SKRect.Create(x, y, width, height), SKRect.Create(0, 0, width, height), new SKSamplingOptions(SKCubicResampler.Mitchell));
            }
            using var png = cropped.Encode(SKEncodedImageFormat.Png, 100);
            return png.ToArray();
        }
    }
}
