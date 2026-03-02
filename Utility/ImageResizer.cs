namespace Devdiscourse.Utility
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Formats;
    using SixLabors.ImageSharp.Formats.Webp;
    using SixLabors.ImageSharp.Processing;

    public class ImageResizer
    {
        private readonly HttpClient _httpClient;

        public ImageResizer(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Stream> ResizeImageFromUrlAsync(string imageUrl, int width = 0, int height = 0, string mode = "crop", string format = "jpeg", int quality = 80)
        {
            try
            {
                using var response = await _httpClient.GetAsync(imageUrl);
                response.EnsureSuccessStatusCode();
                await using var inputStream = await response.Content.ReadAsStreamAsync();
                var outputStream = new MemoryStream();
                using (var image = await Image.LoadAsync(inputStream))
                {
                    if (width == 0 && height == 0)
                    {
                        width = (int)(image.Width * 0.05);
                        height = (int)(image.Height * 0.05);
                    }
                    else if (width == 0)
                    {
                        float aspectRatio = (float)image.Width / image.Height;
                        width = (int)(height * aspectRatio);
                    }
                    else if (height == 0)
                    {
                        float aspectRatio = (float)image.Height / image.Width;
                        height = (int)(width * aspectRatio);
                    }

                    var resizeOptions = new ResizeOptions
                    {
                        Size = new Size(width, height),
                        Mode = mode == "crop" ? ResizeMode.Crop : ResizeMode.Max
                    };
                    image.Mutate(x => x.Resize(resizeOptions));

                    IImageEncoder encoder = format.ToLower() switch
                    {
                        "webp" => new SixLabors.ImageSharp.Formats.Webp.WebpEncoder
                        {
                            Quality = quality,
                            Method = WebpEncodingMethod.Fastest,
                            FileFormat = WebpFileFormatType.Lossy
                        },
                        "jpeg" => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = quality },
                        "png" => new SixLabors.ImageSharp.Formats.Png.PngEncoder(), // PNG does not have quality settings
                        _ => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder { Quality = quality }
                    };

                    await image.SaveAsync(outputStream, encoder);
                    outputStream.Position = 0;
                }
                return outputStream;
            }
            catch (HttpRequestException ex)
            {
                // Log and handle the exception appropriately.
                throw new InvalidOperationException("Error downloading the image.", ex);
            }
            catch (Exception ex)
            {
                // Log and handle other exceptions.
                throw new InvalidOperationException("Error processing the image.", ex);
            }
        }
    }

}
