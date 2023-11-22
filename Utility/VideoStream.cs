using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Net;

namespace Devdiscourse.Utility
{
    public class VideoStream
    {
        private readonly string _filename;
        private readonly string _blobContainerName;
        private readonly CloudBlobContainer blobContainer;
        private readonly CloudBlockBlob blob;
        public long Length { get; set; }
        public long Start { get; set; }
        public long End { get; set; }
        public VideoStream(string filename, string blobContainerName)
        {
            _filename = filename;
            _blobContainerName = blobContainerName;
            blobContainer = GetCloudBlobImageContainer(_blobContainerName);
            blob = blobContainer.GetBlockBlobReference(_filename);
            var sasToken = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddDays(1),//assuming the blob can be downloaded in 10 miinutes
            });
            blob.FetchAttributesAsync();
            Length = (int)blob.Properties.Length;
        }

        public async void WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
        {

            try
            {
                var buffer = new byte[65536];
                using (Stream video = await blob.OpenReadAsync())
                {
                    if (End == -1)
                    {
                        End = blob.Properties.Length;
                    }
                    var position = Start;
                    var bytesLeft = End - Start + 1;
                    video.Position = Start;
                    while (position <= End)
                    {
                        var bytesRead = video.Read(buffer, 0, (int)Math.Min(bytesLeft, buffer.Length));
                        await outputStream.WriteAsync(buffer, 0, bytesRead);
                        position += bytesRead;
                        bytesLeft = End - position + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                // fail silently
            }
            finally
            {
                outputStream.Close();
            }
        }
        private CloudBlobContainer GetCloudBlobImageContainer(string blobContainerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("devdiscourse_AzureStorageConnectionString"));
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(blobContainerName);
            //if (container.CreateIfNotExistsAsync())
            //{
            //    container.SetPermissionsAsync(new BlobContainerPermissions
            //    {
            //        PublicAccess = BlobContainerPublicAccessType.Blob
            //    });
            //}
            bool created = Convert.ToBoolean(container.CreateIfNotExistsAsync());
            if (created)
            {
                container.SetPermissionsAsync(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
            return container;
        }
    }
}
