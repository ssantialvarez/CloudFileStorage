using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CloudFileStorage.Helpers
{
    public class AzureHandler
    {
        public static async Task<string> UploadFileAsync(IFormFile file, string? prefix, string containerName, BlobServiceClient blobServiceClient)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            string blobName = $"{prefix}/{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.UploadAsync(file.OpenReadStream(), true);
            return blobClient.Uri.ToString();
        }
        public static async Task DeleteFileAsync(string blobName, string containerName, BlobServiceClient blobServiceClient)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteAsync();
        }
        public static async Task<BlobDownloadInfo?> DownloadFileAsync(string fileName, string? prefix, string containerName, BlobServiceClient blobServiceClient)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            string blobName = $"{prefix}/{fileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            if (await blobClient.ExistsAsync())
            {
                return await blobClient.DownloadAsync();
            }

            return null;
        }

    }
}
