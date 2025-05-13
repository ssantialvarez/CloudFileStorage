using CloudFileStorage.Models.DTOs;

namespace CloudFileStorage.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetUsersAsync();
        Task<UserResponse> GetMeAsync();
        Task<UserResponse> GetUserByIdAsync(string id);
        Task<UserResponse> UpdateUserAsync(UpdateUserRequest req);
        Task<UserResponse> DeleteUserAsync(string id);
        Task<UserResponse> DeleteOwnUserAsync();
    }
}
