using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace CloudFileStorage.Helpers
{
    public class S3Handler
    {
        // Function to upload a file to an S3 bucket.
        public static async Task<PutObjectResponse> UploadFile(IFormFile file, string bucketName, string? prefix, IAmazonS3 client)
        {
            // Create a PutObjectRequest.
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = string.IsNullOrEmpty(prefix) ? file.FileName : $"{prefix}/{file.FileName}",
                InputStream = file.OpenReadStream()
            };
            request.Metadata.Add("Content-Type", file.ContentType);
            // Upload the file.
            return await client.PutObjectAsync(request);
        }
        public static async Task<FileStreamResult> DownloadFileAsync(string bucketName, string objectKey, IAmazonS3 client)
        {
            var request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            var s3Object = await client.GetObjectAsync(request);

            // Devuelve el archivo como FileStreamResult
            return new FileStreamResult(s3Object.ResponseStream, s3Object.Headers.ContentType)
            {
                FileDownloadName = objectKey
            };
        }

        public static async Task<DeleteObjectResponse> DeleteFileAsync(string bucketName, string objectKey, IAmazonS3 client)
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey
            };

            return await client.DeleteObjectAsync(request);
        }
    }
}
