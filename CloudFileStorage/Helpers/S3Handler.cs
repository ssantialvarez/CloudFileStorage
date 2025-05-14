using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace CloudFileStorage.Helpers
{
    public class S3Handler : IProviderHandler
    {
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 client;
        public S3Handler(IAmazonS3 s3Client, IConfiguration configuration)
        {
            client = s3Client;
            _configuration = configuration;
        }
        // Function to upload a file to an S3 bucket.
        public async Task<bool> UploadFileAsync(IFormFile file, string? prefix)
        {
            var bucketName = _configuration["AWS:BucketName"];

            // Create a PutObjectRequest.
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix}/{file.FileName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            // Upload the file.
            var res = await client.PutObjectAsync(request);
            return res != null;
        }
        public async Task<string> DownloadFileAsync(string fileName, string? prefix)
        {
            var bucketName = _configuration["AWS:BucketName"];
            // returns presigned URL
            var response = await client.GetPreSignedURLAsync(new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = $"{prefix}/{fileName}",
                Expires = DateTime.UtcNow.AddMinutes(5)
            });

            return response;
        }

        public async Task DeleteFileAsync(string fileName, string? prefix)
        {
            var bucketName = _configuration["AWS:BucketName"];
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = $"{prefix}/{fileName}"
            };

            await client.DeleteObjectAsync(request);
        }
    }
}

