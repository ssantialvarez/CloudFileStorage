using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace CloudFileStorage.Helpers
{
    public class AzureHandler : IProviderHandler
    {
        private readonly IConfiguration _configuration;
        private readonly BlobServiceClient _blobServiceClient;
        public AzureHandler(BlobServiceClient blobServiceClient,IConfiguration configuration)
        {
            _blobServiceClient = blobServiceClient;
            _configuration = configuration;
        }
        public async Task<bool> UploadFileAsync(IFormFile file, string? prefix)
        {
            var containerName = _configuration["Azure:ContainerName"];
            var containerClient =_blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            string blobName = $"{prefix}/{file.FileName}";
            var blobClient = containerClient.GetBlobClient(blobName);

            
            var res = await blobClient.UploadAsync(file.OpenReadStream(), true);
            return res != null;
        }
        public async Task DeleteFileAsync(string fileName, string? prefix)
        {
            var containerName = _configuration["Azure:ContainerName"];
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            string blobName = $"{prefix}/{fileName}";
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteAsync();
        }
        public async Task<string> DownloadFileAsync(string fileName, string? prefix)
        {

            var containerName = _configuration["Azure:ContainerName"];
            var accessKey = _configuration["Azure:AccessToken"];

            if (string.IsNullOrWhiteSpace(containerName))
                throw new InvalidOperationException("Azure container name is not configured.");
            if (string.IsNullOrWhiteSpace(accessKey))
                throw new InvalidOperationException("Azure access key is not configured.");

            var blobPath = string.IsNullOrEmpty(prefix) ? fileName : $"{prefix}/{fileName}";

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobPath);

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException("The requested file does not exist in blob storage.");

            var sasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddMinutes(5));
            
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobPath,
                Resource = "b", // 'b' = blob (not container)
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            
            var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(
                blobClient.AccountName, accessKey)).ToString();
            
            return $"{blobClient.Uri}?{sasToken}";
            
        }


    }
}


