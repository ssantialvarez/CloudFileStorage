using System.Collections;
using Amazon.S3;
using Azure.Storage.Blobs;
using CloudFileStorage.Data;
using CloudFileStorage.Helpers;
using CloudFileStorage.Models;
using CloudFileStorage.Models.DTOs;
using CloudFileStorage.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using File = CloudFileStorage.Models.File;

namespace CloudFileStorage.Services.Implementations
{
   
    public class FileService : IFileService
    {
        // Constants
        // 5GB = 5000000000 bytes
        // 5MB = 5000000 bytes

        private readonly double MAX_QUOTA = 5000000000;
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

        public async Task<IEnumerable<StatsResponse>> GetStatsAsync()
        {
            // Implementation for retrieving statistics
            // It will return a list of all the users and the amount of storage they have used during that day.
            var today = DateTime.UtcNow.Date;
            var query = from file in _dbContext.Files
                                           where file.uploadedOn.Date == today
                                          join user in _dbContext.Users 
                                              on file.UserId equals user.id
                                          group file by user into g
                                              select new { User = g.Key, Size = g.Sum(f => f.size) };
            var result = await query.Select(item => new StatsResponse(
                item.Size,
                new UserResponse(
                    item.User.id.ToString(),
                    item.User.username,
                    item.User.role,
                    item.User.createdAt,
                    item.User.updatedAt
                )
            )).ToListAsync();
                    
            return result;
        }
        
        public Task<bool> DeleteFileAsync(Guid id)
        {
            var providers = new List<IProviderHandler>
            {
                new AzureHandler(_blobServiceClient, _configuration),
                new S3Handler(_s3Client,_configuration)
            };
            var userId = _tokenProvider.GetUserIdFromToken();
            var role = _tokenProvider.GetRoleFromToken();
            try
            {
                var file = _dbContext.Files.FirstOrDefault(f => f.id == id && f.UserId.ToString() == userId);
                if (file == null || role != "Admin")
                {
                    throw new KeyNotFoundException("File not found in database or owners incorrect credentials");
                }
                _dbContext.Files.Remove(file);
                _dbContext.SaveChanges();
             

                foreach (var provider in providers)
                {
                    provider.DeleteFileAsync(file.fileName, file.UserId.ToString()).Wait();
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

        public async Task<List<FileResponse>> GetOwnFilesAsync()
        {
            string userId = _tokenProvider.GetUserIdFromToken();
            return await GetFilesByUserIdAsync(userId);
        }

        public async Task<FileResponse> UploadFileAsync(IFormFile file)
        {
            var providers = new List<IProviderHandler>
            {
                new S3Handler(_s3Client, _configuration),
                new AzureHandler(_blobServiceClient, _configuration)
            };
            File? newFile = null;
            double totalSize = 0;
            string userId = _tokenProvider.GetUserIdFromToken();

            // I want to check that the user did not upload more than 5GB THIS month
            // So the idea is to look for all the files he uploaded in the ongoing month
            // SUM the size of all those files and check that he didnt upload more than 5GB
            // If he didnt, then we can upload the file
            List<File> files = await _dbContext.Files.Where(f => f.UserId.ToString() == userId).ToListAsync();

            foreach (File f in files)
            {
                if(f.fileName == file.FileName)
                    throw new Exception("File with the same name already exists");
                
                if(f.uploadedOn.Month == DateTime.UtcNow.Month && f.uploadedOn.Year == DateTime.UtcNow.Year)
                    totalSize += f.size;
            }
            
            // var totalSize = _dbContext.Files.Where(f => f.UserId.ToString() == userId && f.uploadedOn.Month == DateTime.UtcNow.Month && f.uploadedOn.Year == DateTime.UtcNow.Year).Sum(f => f.size);;
            
            if(totalSize + file.Length > MAX_QUOTA)
                throw new Exception("You have reached your monthly limit.");
            
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.");

            try
            {
                foreach (var provider in providers)
                {
                    // Upload the file to the provider
                    var uploadResponse = await provider.UploadFileAsync(file, userId);
                    if (!uploadResponse)
                    {
                        throw new Exception("Error uploading file to provider");
                    }
                }

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

        public async Task<string?> DownloadFileAsync(Guid id)
        {
            var providers = new List<IProviderHandler>
            {
                new S3Handler(_s3Client, _configuration),
                new AzureHandler(_blobServiceClient, _configuration)
            };
            var file = await _dbContext.Files
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.id == id);
            if (file == null)
            {
                throw new KeyNotFoundException("File not found in database");
            }
            try
            {
                foreach (var provider in providers)
                {
                    var result = await provider.DownloadFileAsync(file.fileName, file.UserId.ToString());
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading file", ex);
            }
            return null;
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



