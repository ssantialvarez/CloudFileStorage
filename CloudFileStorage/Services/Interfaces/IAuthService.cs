using CloudFileStorage.Models.DTOs;

namespace CloudFileStorage.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(AuthRequest req);
        Task<AuthResponse> RegisterAsync(AuthRequest req);
    }
}
