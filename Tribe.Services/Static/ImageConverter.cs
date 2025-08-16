using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Formats;
using System.IO;

namespace Tribe.Services.Static
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Formats.Webp;
    using SixLabors.ImageSharp.Formats.Png;
    using SixLabors.ImageSharp.Formats.Jpeg;
    using SixLabors.ImageSharp.Processing;
    using SixLabors.ImageSharp.Processing.Processors.Transforms;
    using System.IO;



public static class ImageConverter
    {
        /// <summary>
        /// Konvertiert einen Base64-String in ein Zielformat, skaliert und gibt das Ergebnis als Base64-String zurück.
        /// </summary>
        /// <param name="base64String">Der Base64-String des Quellbildes.</param>
        /// <param name="targetFormat">Das gewünschte Zielformat ("webp", "png", "jpeg"). Standard ist "webp".</param>
        /// <param name="quality">Das Qualitätslevel für die Kompression (1-100), relevant für JPEG und WebP.</param>
        /// <param name="size">Die maximale Größe für das Bild. Das Bild wird proportional zugeschnitten.</param>
        /// <returns>Ein Base64-String im Zielformat mit dem passenden Data-URL-Präfix.</returns>
        public static string ConvertAndResizeBase64(string base64String, int quality = 75, Size? size = null, string? targetFormat = "webp")
        {
            // 1. Validierung
            var parts = base64String.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Eingabe-String ist keine gültige Daten-URL.");
            }
            var base64Data = parts[1];
            var imageBytes = Convert.FromBase64String(base64Data);

            // 2. Lade das Bild
            using var image = Image.Load(imageBytes);

            // 3. Zuschneiden und Skalieren, falls eine Größe angegeben ist
            if (size.HasValue && (image.Width > size.Value.Width || image.Height > size.Value.Height))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = size.Value,
                    Mode = ResizeMode.Crop,
                    Sampler = new BicubicResampler()
                }));
            }

            // 4. Encoder auswählen basierend auf dem Zielformat
            IImageEncoder encoder;
            string mimeType;

            switch (targetFormat?.ToLower())
            {
                case "webp":
                    encoder = new WebpEncoder { Quality = quality };
                    mimeType = "image/webp";
                    break;
                case "png":
                    encoder = new PngEncoder();
                    mimeType = "image/png";
                    break;
                case "jpeg":
                    encoder = new JpegEncoder { Quality = quality };
                    mimeType = "image/jpeg";
                    break;
                default:
                    throw new ArgumentException($"Das Format '{targetFormat}' wird nicht unterstützt.");
            }

            // 5. Bild konvertieren und als Stream speichern
            using var outputStream = new MemoryStream();
            image.Save(outputStream, encoder);

            // 6. Rückgabe des Base64-Strings mit passendem Präfix
            var outputBase64Data = Convert.ToBase64String(outputStream.ToArray());
            return $"data:{mimeType};base64,{outputBase64Data}";
        }
    }
}
