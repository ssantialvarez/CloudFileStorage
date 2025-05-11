using CloudFileStorage.Data;
using CloudFileStorage.Helpers;
using CloudFileStorage.Models;
using CloudFileStorage.Models.DTOs;
using CloudFileStorage.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloudFileStorage.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserContext _context;
        private readonly TokenProvider _tokenProvider;
        private readonly PasswordHasher _passwordHasher;

        public AuthService(UserContext context, TokenProvider tokenProvider, PasswordHasher passwordHasher)
        {
            _context = context;
            _tokenProvider = tokenProvider;
            _passwordHasher = passwordHasher;
        }
        public async Task<AuthResponse> LoginAsync(AuthRequest req)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.username == req.username);
            if (user is null)
            {
                return new AuthResponse
                {
                    success = false,
                    message = "Invalid username."
                };
            }
            bool verified = _passwordHasher.Verify(req.password, user.password);
            if (!verified)
            {
                return new AuthResponse
                {
                    success = false,
                    message = "Invalid password."
                };
            }
            return new AuthResponse
            {
                success = true,
                token = _tokenProvider.Create(user),
                message = "Login successful"
            };
        }

        public async Task<AuthResponse> RegisterAsync(AuthRequest req)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.username == req.username);
            if (user is not null)
            {
                return new AuthResponse
                {
                    success = false,
                    message = "Username already exists."
                };
            }
            user = new User
            {
                id = Guid.NewGuid().ToString(),
                role = UserRole.User,
                username = req.username,
                password = _passwordHasher.Hash(req.password)
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return new AuthResponse
            {
                success = true,
                token = _tokenProvider.Create(user),
                message = "Registration successful"
            };
        }
    }
}
