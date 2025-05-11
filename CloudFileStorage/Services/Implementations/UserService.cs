using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CloudFileStorage.Data;
using CloudFileStorage.Models;
using CloudFileStorage.Models.DTOs;
using CloudFileStorage.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudFileStorage.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(UserContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public async Task<UserResponse> DeleteUserAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == id);
            if (user == null)
                throw new KeyNotFoundException("User not found in database");
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new UserResponse
            {
                id = user.id,
                username = user.username,
                role = user.role,
                createdAt = user.createdAt,
                updatedAt = user.updatedAt
            };
        }
        public async Task<UserResponse> GetMeAsync()
        {
            var userId = GetUserIdFromToken();

            var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);

            if (userEntity == null)
                throw new KeyNotFoundException("User not found in database");

            return new UserResponse
            {
                id = userEntity.id,
                username = userEntity.username,
                role = userEntity.role,
                createdAt = userEntity.createdAt,
                updatedAt = userEntity.updatedAt
            };
        }
        public async Task<UserResponse> GetUserByIdAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == id);
            if (user == null)
                throw new KeyNotFoundException("User not found in database");
            return new UserResponse
            {
                id = user.id,
                username = user.username,
                role = user.role,
                createdAt = user.createdAt,
                updatedAt = user.updatedAt
            };
        }
        public async Task<List<UserResponse>> GetUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(users => new UserResponse
            {
                id = users.id,
                username = users.username,
                role = users.role,
                createdAt = users.createdAt,
                updatedAt = users.updatedAt
            }).ToList();
        }
        public async Task<UserResponse> UpdateUserAsync(UpdateUserRequest req)
        {
            string userId = GetUserIdFromToken();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.id == userId);
            if (user == null)
                throw new KeyNotFoundException("User not found in database");
            
            user.username = req.username ?? user.username;
        
            user.password = req.password ?? user.password;
            
            user.updatedAt = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new UserResponse
            {
                id = user.id,
                username = user.username,
                role = user.role,
                createdAt = user.createdAt,
                updatedAt = user.updatedAt
            };

        }

        private string GetUserIdFromToken()
        {
            var user = _contextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User not authenticated");
            var userId = user.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found in token");
            return userId;
        }
    }
}
