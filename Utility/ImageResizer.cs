namespace Devdiscourse.Utility
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ImageResizer
    {
        private readonly HttpClient HttpClient = new();

        public async Task<Stream> ResizeImageFromUrlAsync(string imageUrl, int width = 0, int height = 0, string mode = "resize", string format = "jpeg")
        {
            using var response = await HttpClient.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();
            using var inputStream = await response.Content.ReadAsStreamAsync();
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

                if (mode == "crop")
                {
                    var resizeOptions = new ResizeOptions
                    {
                        Size = new Size(width, height),
                        Mode = ResizeMode.Crop
                    };
                    image.Mutate(x => x.Resize(resizeOptions));
                }
                else
                {
                    image.Mutate(x => x.Resize(width, height));
                }

                switch (format.ToLower())
                {
                    case "webp":
                        await image.SaveAsWebpAsync(outputStream);
                        break;
                    case "jpeg":
                    default:
                        await image.SaveAsJpegAsync(outputStream);
                        break;
                    case "png":
                        await image.SaveAsPngAsync(outputStream);
                        break;
                }
                outputStream.Position = 0;
            }
            return outputStream;
        }
    }
}
