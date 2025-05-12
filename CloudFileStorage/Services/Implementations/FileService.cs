using Amazon.S3;
using Azure.Storage.Blobs;
using CloudFileStorage.Data;
using CloudFileStorage.Helpers;
using CloudFileStorage.Models.DTOs;
using CloudFileStorage.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using File = CloudFileStorage.Models.File;

namespace CloudFileStorage.Services.Implementations
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly TokenProvider _tokenProvider;
        private readonly IAmazonS3 _s3Client;
        private readonly BlobServiceClient _blobServiceClient;

        public FileService(
            IConfiguration configuration,
            ApplicationDbContext dbContext,
            TokenProvider tokenProvider,
            IAmazonS3 s3Client,
            BlobServiceClient blobServiceClient)
        {
            _dbContext = dbContext;
            _tokenProvider = tokenProvider;
            _configuration = configuration;
            _s3Client = s3Client;
            _blobServiceClient = blobServiceClient;
        }
        public Task<bool> DeleteFileAsync(Guid id)
        {
            var userId = _tokenProvider.GetUserIdFromToken();
            try
            {
                var file = _dbContext.Files.FirstOrDefault(f => f.id == id && f.UserId.ToString() == userId);
                if (file == null)
                {
                    throw new KeyNotFoundException("File not found in database or owners incorrect credentials");
                }
                _dbContext.Files.Remove(file);
                _dbContext.SaveChanges();
                var bucketName = _configuration["AWS:BucketName"];
                var objectKey = $"{file.UserId}/{file.fileName}";
                // Delete the file from S3
                S3Handler.DeleteFileAsync(bucketName, objectKey, _s3Client);
                // Delete the file from Azure Blob Storage
                AzureHandler.DeleteFileAsync(objectKey, _configuration["Azure:ContainerName"], _blobServiceClient);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting file", ex);
            }
        }

        public async Task<FileResponse> GetFileByIdAsync(Guid fileId)
        {
            // Implementation for retrieving a file by its ID
            var file = await _dbContext.Files
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.id == fileId);
            if (file == null)
            {
                throw new KeyNotFoundException("File not found in database");
            }
            return new FileResponse
            {
                id = file.id,
                fileName = file.fileName,
                size = file.size,
                contentType = file.contentType,
                uploadedOn = file.uploadedOn,
                UserId = file.UserId,
            };
        }

        public async Task<List<FileResponse>> GetFilesByUserIdAsync(string userId)
        {
            // Implementation for retrieving files by user ID
            var files = await _dbContext.Files
                .Where(f => f.UserId.ToString() == userId)
                .Select(f => new FileResponse
                {
                    id = f.id,
                    fileName = f.fileName,
                    size = f.size,
                    contentType = f.contentType,
                    uploadedOn = f.uploadedOn,
                    UserId = f.UserId,
                }).ToListAsync();
            return files;
        }

        public async Task<FileResponse> UploadFileAsync(IFormFile file)
        {
            string userId = _tokenProvider.GetUserIdFromToken();
            string bucketName = _configuration["AWS:bucketName"];
         
            File? newFile = null;

            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.");
            
            try
            {
                // Upload the file to S3
                object s3UploadResponse = await S3Handler.UploadFileAsync(file, bucketName, userId, _s3Client);
                // Upload the file to Azure Blob Storage
                string azureUploadResponse = await AzureHandler.UploadFileAsync(file, userId, _configuration["Azure:ContainerName"], _blobServiceClient);

                // Create a new File object
                newFile = new File
                {
                    id = Guid.NewGuid(),
                    fileName = file.FileName,
                    size = file.Length,
                    contentType = file.ContentType,
                    UserId = new Guid(userId)
                };
                // Save the file information to the database
                _dbContext.Files.Add(newFile);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error uploading file to S3", ex);
            }

            return new FileResponse
            {
                id = newFile.id,
                fileName = newFile.fileName,
                size = newFile.size,
                contentType = newFile.contentType,
                uploadedOn = newFile.uploadedOn,
                UserId = newFile.UserId,
            };
        }

        public async Task<IActionResult> DownloadFileAsync(Guid id)
        {
            var file = await _dbContext.Files
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.id == id);
            if (file == null)
            {
                throw new KeyNotFoundException("File not found in database");
            }
            try
            {
                var bucketName = _configuration["AWS:BucketName"];
                var result = await S3Handler.DownloadFileAsync(bucketName, file.fileName, file.UserId.ToString(), _s3Client);
                // If the file is not found in S3, check Azure Blob Storage
                
                if (result == null)
                {
                    var downloadResponse = await AzureHandler.DownloadFileAsync(file.fileName, file.UserId.ToString(), _configuration["Azure:ContainerName"], _blobServiceClient);
                    if (downloadResponse != null)
                    {
                        return new FileStreamResult(downloadResponse.Content, file.contentType)
                        {
                            FileDownloadName = file.fileName
                        };
                    }
                    throw new KeyNotFoundException("File not found in both S3 and Azure Blob Storage");
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading file", ex);
            }
        }

        public async Task<List<FileResponse>> GetAllFilesAsync()
        {
            // Implementation for retrieving all files
            var files = await _dbContext.Files
                .Select(f => new FileResponse
                {
                    id = f.id,
                    fileName = f.fileName,
                    size = f.size,
                    contentType = f.contentType,
                    uploadedOn = f.uploadedOn,
                    UserId = f.UserId,
                }).ToListAsync();
            return files;
        }

    }
}
