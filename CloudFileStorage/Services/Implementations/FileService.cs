using Amazon.S3;
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

        public FileService(
            IConfiguration configuration,
            ApplicationDbContext dbContext,
            TokenProvider tokenProvider,
            IAmazonS3 s3Client)
        {
            _dbContext = dbContext;
            _tokenProvider = tokenProvider;
            _configuration = configuration;
            _s3Client = s3Client;
        }
        public Task<bool> DeleteFileAsync(Guid id)
        {
            try
            {
                var file = _dbContext.Files.FirstOrDefault(f => f.id == id);
                if (file == null)
                {
                    throw new KeyNotFoundException("File not found in database");
                }
                _dbContext.Files.Remove(file);
                _dbContext.SaveChanges();
                var bucketName = _configuration["AWS:BucketName"];
                var objectKey = $"{file.UserId}/{file.fileName}";
                var deleteResponse = S3Handler.DeleteFileAsync(bucketName, objectKey, _s3Client);
                if (deleteResponse == null)
                {
                    throw new Exception("Error deleting file from S3");
                }
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
            return new FileResponse
            {
                id = file.id,
                fileName = file.fileName,
                size = file.size,
                contentType = file.contentType,
                uploadedOn = file.uploadedOn,
                UserId = file.UserId,
            } ?? throw new KeyNotFoundException("File not found in database");
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
            // Implementation for uploading a file
            string userId = _tokenProvider.GetUserIdFromToken();
            string bucketName = _configuration["AWS:bucketName"];
         
            File? newFile = null;

            // Validate the file
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty.");
            }
            
            try
            {
                // Upload the file to S3
                object s3UploadResponse = await S3Handler.UploadFile(file, bucketName, userId, _s3Client);

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
            // Implementation for downloading a file
            var file = await _dbContext.Files
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.id == id);
            if (file == null)
            {
                throw new KeyNotFoundException("File not found in database");
            }
            var bucketName = _configuration["AWS:BucketName"];
            var result = await S3Handler.DownloadFileAsync(bucketName, $"{file.UserId}/{file.fileName}", _s3Client);
            return result;

        }   

    }
}
