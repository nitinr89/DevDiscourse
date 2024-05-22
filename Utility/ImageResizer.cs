namespace Devdiscourse.Utility
{
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class ImageResizer
    {
        private readonly HttpClient HttpClient = new();

        public async Task<Stream> ResizeImageFromUrlAsync(string imageUrl, int width, int height)
        {
            using var response = await HttpClient.GetAsync(imageUrl);
            response.EnsureSuccessStatusCode();
            using var inputStream = await response.Content.ReadAsStreamAsync();
            var outputStream = new MemoryStream();
            using (var image = await Image.LoadAsync(inputStream))
            {
                image.Mutate(x => x.Resize(width, height));
                await image.SaveAsJpegAsync(outputStream);
                outputStream.Position = 0;
            }
            return outputStream;
        }
    }
}