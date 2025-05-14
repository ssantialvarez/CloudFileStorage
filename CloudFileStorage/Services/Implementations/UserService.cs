using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CloudFileStorage.Data;
using CloudFileStorage.Helpers;
using CloudFileStorage.Models;
using CloudFileStorage.Models.DTOs;
using CloudFileStorage.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using File = CloudFileStorage.Models.File;
using Amazon.S3;
using Azure.Storage.Blobs;
namespace CloudFileStorage.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TokenProvider _tokenProvider;
        private readonly PasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IAmazonS3 _s3Client;
        private readonly BlobServiceClient _blobServiceClient;
        

        public UserService(
            ApplicationDbContext context, 
            IHttpContextAccessor contextAccessor, 
            TokenProvider tokenProvider, 
            PasswordHasher passwordHasher,
            IConfiguration configuration,
            IAmazonS3 s3Client,
            BlobServiceClient blobServiceClient)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _tokenProvider = tokenProvider;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _s3Client = s3Client;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<UserResponse> DeleteUserAsync(string id)
        {
            var bucketName = _configuration["AWS:BucketName"];
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id.ToString() == id);
            if (user == null)
                throw new KeyNotFoundException("User not found in database");
            
            // Fetch all files associated with the user
            var files = await _context.Files.Where(f => f.UserId.ToString() == id).ToListAsync();

            try
            {
                foreach (File fileToDelete in files)
                {
                    var objectKey = $"{fileToDelete.UserId}/{fileToDelete.fileName}";
                    // Delete the file from S3
                    S3Handler.DeleteFileAsync(bucketName, objectKey, _s3Client);
                    // Delete the file from Azure Blob Storage
                    AzureHandler.DeleteFileAsync(objectKey, _configuration["Azure:ContainerName"], _blobServiceClient);    
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting user and its files.", ex);
            }
            
            
            return new UserResponse
            (
                user.id.ToString(),
                user.username,
                user.role,
                user.createdAt,
                user.updatedAt
            );
        }
        public async Task<UserResponse> GetMeAsync()
        {
            var userId = _tokenProvider.GetUserIdFromToken();

            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.id.ToString() == userId);

            if (userEntity == null)
                throw new KeyNotFoundException("User not found in database");

            return new UserResponse
            (
                userEntity.id.ToString(),
                userEntity.username,
                userEntity.role,
                userEntity.createdAt,
                userEntity.updatedAt
            );
        }
        public async Task<UserResponse> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id.ToString() == id);
            if (user == null)
                throw new KeyNotFoundException("User not found in database");
            return new UserResponse
            (
                user.id.ToString(),
                user.username,
                user.role,
                user.createdAt,
                user.updatedAt
            );
        }
        public async Task<List<UserResponse>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(users => new UserResponse
            (
                users.id.ToString(),
                users.username,
                users.role,
                users.createdAt,
                users.updatedAt
            )).ToList();
        }

        public Task<UserResponse> DeleteOwnUserAsync()
        {
            var userId = _tokenProvider.GetUserIdFromToken();
            return DeleteUserAsync(userId);
        }

        public async Task<UserResponse> UpdateUserAsync(UpdateUserRequest req)
        {
            string userId = _tokenProvider.GetUserIdFromToken();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id.ToString() == userId);
            if (user == null)
                throw new KeyNotFoundException("User not found in database");
            
            user.username = req.username ?? user.username;
        
            if(!string.IsNullOrWhiteSpace(req.password))
                user.password = _passwordHasher.Hash(req.password);              
            
            if (!string.IsNullOrWhiteSpace(req.role) && 
                Enum.TryParse<UserRole>(req.role, ignoreCase: true, out var parsedRole) &&
                Enum.IsDefined(typeof(UserRole), parsedRole))
            {
                user.role = parsedRole;
            }
            
            
            user.updatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new UserResponse
            (
                user.id.ToString(),
                user.username,
                user.role,
                user.createdAt,
                user.updatedAt
            );

        }
    }
}